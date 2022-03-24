using System.IdentityModel.Tokens.Jwt;
using Data.Collections;
using Microsoft.IdentityModel.Tokens;
using Settings;
using Utils.Security;

namespace Utils.Jwt;

public class JwtManager
{
    private readonly GenerateTokens GenerateTokens;
    private readonly SigningConfigurations SigningConfigurations;
    public JwtManager(GenerateTokens generateTokens, SigningConfigurations signingConfigurations)
    {
        GenerateTokens = generateTokens;
        SigningConfigurations = signingConfigurations;
    }
    public Task<JwtAuthResponse> Authenticate(UserCollection user)
    {
        var stamp = StampGenerator.Generate();
        return Task.FromResult(new JwtAuthResponse
        {
            AccessToken = GenerateTokens.Access(user, stamp),
            RefreshToken = GenerateTokens.Refresh(user, stamp)
        });
    }

    public Task<bool> CompareStamps(string accessToken, string refreshToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var accessSecurityToken = handler.ReadJwtToken(accessToken);
        var refreshSecurityToken = handler.ReadJwtToken(refreshToken);
        var accessStamp = accessSecurityToken.Claims.First(claim => claim.Type == "stamp").Value;
        var refreshStamp = refreshSecurityToken.Claims.First(claim => claim.Type == "stamp").Value;
        return accessStamp == refreshStamp ? Task.FromResult(true) : Task.FromResult(false);
    }

    public Task<bool> IsValidAccess(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            tokenHandler.ValidateToken(token, AccessValidationParameters(), out validatedToken);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private TokenValidationParameters AccessValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningConfigurations.Key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidIssuer = JwtSettings.Issuer,
            ValidAudience = JwtSettings.Audience,
        };
    }

    public Task<bool> IsValidRefresh(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;
            tokenHandler.ValidateToken(token, RefreshValidationParameters(), out validatedToken);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    private TokenValidationParameters RefreshValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = SigningConfigurations.Key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = JwtSettings.Issuer,
            ValidAudience = JwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };
    }
}

public class JwtAuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}