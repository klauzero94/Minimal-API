using Data;
using Data.Collections;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Outputs;
using Utils;
using Utils.Security;

namespace Routes;

public class SeedRoutes
{
    private readonly ConnectionFactory ConnectionFactory;
    private readonly IMongoCollection<UserCollection> User;
    private readonly PasswordHasher PasswordHasher;
    public SeedRoutes(PasswordHasher passwordHasher)
    {
        ConnectionFactory = new ConnectionFactory();
        User = ConnectionFactory.Collection<UserCollection>();
        PasswordHasher = passwordHasher;
    }

    public async Task MapActionsAsync(WebApplication app)
    {
        #region Seed God User
        app.MapPost(APIPaths.Seed.GodUser, [AllowAnonymous] async () => 
        {
            using (var session = await ConnectionFactory.Client().StartSessionAsync())
            {
                if (await User.Find(x => x.Role == UserRoles.God).AnyAsync())
                    return Results.NotFound(new ResponseBase(
                        "404", false, error: new ErrorOutput { Message = "Super usuário já existe." }
                    ));
                var salt = PasswordHasher.GenerateSalt();
                var user = new UserCollection()
                {
                    FullName = "Super Usuário",
                    Email = "super@email.com",
                    Username = "super",
                    PasswordSalt = salt,
                    PasswordHash = PasswordHasher.GenerateHash("super", salt),
                    Active = true
                };
                await User.InsertOneAsync(session, user);
                return Results.Ok(new ResponseBase("200", true));
            }
        })
        .Produces<ResponseBase>(StatusCodes.Status200OK)
        .Produces<ResponseBase<object, ErrorOutput>>(StatusCodes.Status404NotFound)
        .WithName("GodUser")
        .WithTags("Seed");
        #endregion
        
        await Task.CompletedTask;
    }
}