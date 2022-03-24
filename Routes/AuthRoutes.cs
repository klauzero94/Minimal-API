using Data;
using Data.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Models.User;
using MongoDB.Driver;
using Outputs;
using Utils;
using Utils.Caching;
using Utils.Email;
using Utils.Jwt;
using Utils.Security;

namespace Routes;
public class AuthRoutes
{
    private readonly ConnectionFactory ConnectionFactory;
    private readonly IMongoCollection<UserCollection> User;
    private readonly PasswordHasher PasswordHasher;
    private readonly JwtManager JwtManager;
    private readonly Cache Cache;
    private readonly EmailSender EmailSender;

    public AuthRoutes(PasswordHasher passwordHasher, JwtManager jwtManager, Cache cache, EmailSender emailSender)
    {
        ConnectionFactory = new ConnectionFactory();
        User = ConnectionFactory.Collection<UserCollection>();
        PasswordHasher = passwordHasher;
        JwtManager = jwtManager;
        Cache = cache;
        EmailSender = emailSender;
    }

    public async Task MapActionsAsync(WebApplication app)
    {
        #region Login
        app.MapPost(APIPaths.Auth.Login, [AllowAnonymous] async (
            [FromBody] LogInModel model) => 
        {

            var arrayFilter = Builders<UserCollection>.Filter.Eq(x => x.Email, model.UsernameOrEmail.ToLower())
                | Builders<UserCollection>.Filter.Eq(x => x.Username, model.UsernameOrEmail.ToLower());
            var user = User.Find(arrayFilter);
            if (await user.AnyAsync())
            {
                var _user = await user.SingleOrDefaultAsync();
                if (_user.PasswordHash == PasswordHasher.GenerateHash(model.Password, _user.PasswordSalt))
                    return Results.Created(APIPaths.Auth.Login, new ResponseBase(
                        "201", true, data: await JwtManager.Authenticate(_user)
                    ));
                return Results.NotFound(new ResponseBase(
                    "404", false, error: new ErrorOutput { Message = "Username, e-mail e / ou senha incorreto(as)." }
                ));
            }
            return Results.NotFound(new ResponseBase(
                "404", false, error: new ErrorOutput { Message = "Username, e-mail e / ou senha incorreto(as)." }
            ));
        })
        .Produces<ResponseBase<JwtAuthResponse, object>>(StatusCodes.Status201Created)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status404NotFound)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status500InternalServerError)
        .WithName("Login")
        .WithTags("Auth");
        #endregion

        #region Refresh
        app.MapPost(APIPaths.Auth.Refresh, [AllowAnonymous] async (
            [FromBody] RefreshModel model,
            HttpContext httpContext) => 
        {
            if (await JwtManager.CompareStamps(httpContext.Request.Headers[HeaderNames.Authorization].First().Substring(7), model.RefreshToken) &&
                    await JwtManager.IsValidAccess(httpContext.Request.Headers[HeaderNames.Authorization].First().Substring(7)) &&
                    await JwtManager.IsValidRefresh(model.RefreshToken))
            {
                var arrayFilter = Builders<UserCollection>.Filter.Eq(x => x.Username, JwtDecoder.GetValueByProperty(httpContext.Request.Headers[HeaderNames.Authorization].First().Substring(7), "email").ToLower())
                | Builders<UserCollection>.Filter.Eq(x => x.Email, JwtDecoder.GetValueByProperty(httpContext.Request.Headers[HeaderNames.Authorization].First().Substring(7), "email").ToLower());
                var user = User.Find(arrayFilter);
                if (!await user.AnyAsync())
                {
                    return Results.NotFound(new ResponseBase(
                        "404", false, error: new ErrorOutput { Message = "Username, e-mail e / ou senha incorreto(as)." }
                    ));
                }
                var _user = await user.SingleOrDefaultAsync();
                return Results.Created(APIPaths.Auth.Login, new ResponseBase(
                    "201", true, data: await JwtManager.Authenticate(_user)
                ));
            }
            return Results.BadRequest(new ResponseBase(
                "400", false, error: new ErrorOutput { Message = "Token de acesso e / ou refresh inválido(s)." }
            ));
        })
        .Produces<ResponseBase<JwtAuthResponse, object>>(StatusCodes.Status201Created)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status400BadRequest)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status404NotFound)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status500InternalServerError)
        .WithName("Refresh")
        .WithTags("Auth");
        #endregion

        #region ForgotPassword
        app.MapPost(APIPaths.Auth.ForgotPassword, [AllowAnonymous] async (
            [FromBody] ForgotPasswordModel model) => 
        {
            var arrayFilter = Builders<UserCollection>.Filter.Eq(x => x.Email, model.UsernameOrEmail.ToLower())
                | Builders<UserCollection>.Filter.Eq(x => x.Username, model.UsernameOrEmail.ToLower());
            var user = User.Find(arrayFilter);
            if (await user.AnyAsync())
            {
                var _user = await user.SingleOrDefaultAsync();
                var resetPasswordPIN = RandomChain.GeneratePIN();
                await Cache.SetCacheValueAsync(_user.Email.ToLower() + "_fp", resetPasswordPIN, 180);
                await EmailSender.SendForgotPasswordAsync(_user.Email.ToLower(), resetPasswordPIN);
            }
            return Results.Ok(new ResponseBase("200", true));
        })
        .Produces<ResponseBase>(StatusCodes.Status200OK)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status500InternalServerError)
        .WithName("ForgotPassword")
        .WithTags("Auth");
        #endregion

        #region ResetPassword
        app.MapPut(APIPaths.Auth.ResetPassword, [AllowAnonymous] async (
            [FromBody] ResetPasswordModel model) => 
        {
            using (var session = await ConnectionFactory.Client().StartSessionAsync())
            {
                var arrayFilter = Builders<UserCollection>.Filter.Eq(x => x.Email, model.UsernameOrEmail.ToLower())
                    | Builders<UserCollection>.Filter.Eq(x => x.Username, model.UsernameOrEmail.ToLower());
                var user = User.Find(arrayFilter);
                if (await user.AnyAsync())
                {
                    var _user = await user.SingleOrDefaultAsync();
                    if (await Cache.GetCacheValueAsync(_user.Email.ToLower() + "_fp") == model.Pin)
                    {
                        _user.PasswordSalt = PasswordHasher.GenerateSalt();
                        _user.PasswordHash = PasswordHasher.GenerateHash(model.Password, _user.PasswordSalt);
                        _user.UpdatedAt = DateTime.UtcNow;
                        await User.ReplaceOneAsync(session, x => x.Id == _user.Id, _user);
                        await Cache.RemoveCacheValueAsync(_user.Email.ToLower() + "_fp");
                    }
                    else
                    return Results.BadRequest(new ResponseBase(
                        "400", false, error: new ErrorOutput { Message = "PIN incorreto." }
                    ));
                }
                else
                    return Results.NotFound(new ResponseBase(
                        "404", false, error: new ErrorOutput { Message = "Username ou e-mail incorreto." }
                    ));
                return Results.Ok(new ResponseBase("200", true));
            }
        })
        .Produces<ResponseBase>(StatusCodes.Status200OK)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status500InternalServerError)
        .WithName("ResetPassword")
        .WithTags("Auth");
        #endregion

        await Task.CompletedTask;
    }
}