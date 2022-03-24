using Extensions;
using Routes;
using Utils;
using Utils.Caching;
using Utils.Email;
using Utils.Security;

var builder = WebApplication.CreateBuilder(args);

#region SetApiExplorer
builder.Services.AddEndpointsApiExplorer();
#endregion

#region Injections
JwtExtension.AddJwt(builder.Services);
builder.Services.AddCors();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<Cache>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<SeedRoutes>();
builder.Services.AddScoped<AuthRoutes>();
builder.Services.AddScoped<UserRoutes>();
SwaggerExtension.AddSwagger(builder.Services);
#endregion

var app = builder.Build();

#region App Uses
SwaggerExtension.SetSwagger(app);
app.UseHttpsRedirection();
app.UseCors(x => x
    .AllowAnyMethod()
    .WithExposedHeaders(HeadersProp.XTotalCount)
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());
app.UseUnauthorizedResponse();
app.UseAuthentication();
app.UseAuthorization();
#endregion

#region Service Scope
using var serviceScope = app.Services.CreateScope();
var services = serviceScope.ServiceProvider;
#endregion

#region Service Instances
await services.GetRequiredService<SeedRoutes>().MapActionsAsync(app);
await services.GetRequiredService<AuthRoutes>().MapActionsAsync(app);
await services.GetRequiredService<UserRoutes>().MapActionsAsync(app);
#endregion

await app.RunAsync();