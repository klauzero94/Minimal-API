using Data;
using Data.Collections;
using MongoDB.Driver;
using Utils;

namespace Routes;

public class UserRoutes
{
    private readonly ConnectionFactory ConnectionFactory;
    private readonly IMongoCollection<UserCollection> User;
    public UserRoutes()
    {
        ConnectionFactory = new ConnectionFactory();
        User = ConnectionFactory.Collection<UserCollection>();
    }

    // Exemplo de uso de custom authorization com retorno de header
    public async Task MapActionsAsync(WebApplication app)
    {
        app.MapGet(APIPaths.User.GetAll, [CustomAuthorize(UserRoles.God)] async (HttpContext httpContext) => 
        {
            await httpContext.Response.WriteAsync(""); // remover, colocado apenas para remover warning em teste.
            httpContext.Response.Headers.Add(HeadersProp.XTotalCount, 5.ToString());
            return Results.Ok(new ResponseBase(
                "200", true
            ));
        });

        await Task.CompletedTask;
    }
}