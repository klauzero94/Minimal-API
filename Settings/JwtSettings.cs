namespace Settings;

public static class JwtSettings
{
    public static int AccessTokenExpire { get; } = 120;
    public static int RefreshTokenExpire { get; } = 60 * 60 * 25 * 30;
    public static string JwtSigningKey { get; } = "substituir_com_sua_chave_secreta";
    public static string Issuer = "";
    public static string Audience = "";
}