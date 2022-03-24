using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data.Collections;
using Microsoft.IdentityModel.Tokens;
using Settings;

namespace Utils.Jwt;

public class GenerateTokens
{
    private SigningConfigurations SigningConfigurations;
    public GenerateTokens(SigningConfigurations signingConfigurations) => SigningConfigurations = signingConfigurations;
    public string Access(UserCollection user, string tokenStamp)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new List<Claim>
        {
            new Claim("uid", user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("username", user.Username),
            new Claim("stamp", tokenStamp),
            new Claim(ClaimTypes.Role, Enum.GetName(typeof(UserRoles), user.Role) ?? "")
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = JwtSettings.Audience,
            Issuer = JwtSettings.Issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(JwtSettings.AccessTokenExpire)),
            SigningCredentials = SigningConfigurations.SigningCredentials
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
    public string Refresh(UserCollection user, string tokenStamp)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var claims = new List<Claim>
        {
            new Claim("uid", user.Id.ToString()),
            new Claim("stamp", tokenStamp)
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = JwtSettings.Audience,
            Issuer = JwtSettings.Issuer,
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(JwtSettings.RefreshTokenExpire)),
            SigningCredentials = SigningConfigurations.SigningCredentials
        };
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }
}

public class SigningConfigurations
{
    public SecurityKey Key { get; }
    public SigningCredentials SigningCredentials { get; }
    public SigningConfigurations()
    {
        var key = Encoding.ASCII.GetBytes(JwtSettings.JwtSigningKey);
        Key = new SymmetricSecurityKey(key);
        SigningCredentials = new SigningCredentials(
            Key, SecurityAlgorithms.HmacSha256Signature);
    }
}