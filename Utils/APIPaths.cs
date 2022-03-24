namespace Utils;

public class APIPaths
{
    public static class Seed
    {
        public const string GodUser = "/seed/users/god";
    }
    public static class Auth
    {
        public const string Login = "/auth/login";
        public const string Refresh = "/auth/refresh";
        public const string ForgotPassword = "/auth/forgot-password";
        public const string ResetPassword = "/auth/reset-password";
    }
    public static class User
    {
        public const string GetAll = "/users";
    }
}