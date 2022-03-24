using System.IdentityModel.Tokens.Jwt;

namespace Utils.Jwt;

public static class JwtDecoder
{
    public static string GetValueByProperty(string token, string propertyName)
    {
        var stream = token;
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(stream);
        var tokenS = jsonToken as JwtSecurityToken;
        return tokenS!.Claims.First(claim => claim.Type == propertyName).Value;
    }
}