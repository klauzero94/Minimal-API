using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Settings;
using Utils.Jwt;

namespace Extensions;

public static class JwtExtension
{
    public static void AddJwt(this IServiceCollection services)
    {
        var signingConfigurations = new SigningConfigurations();
        services.AddScoped<JwtManager>();
        services.AddScoped<GenerateTokens>();
        services.AddScoped<SigningConfigurations>();
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x => {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingConfigurations.Key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = JwtSettings.Issuer,
                ValidAudience = JwtSettings.Audience
            };
        });
        services.AddAuthorization();
    }
}