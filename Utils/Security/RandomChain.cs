namespace Utils.Security;

public static class RandomChain
{
    private static Random? random;
    public static string RandomString(int length)
    {
        random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string RandomAlphanumericString(int length)
    {
        random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return new string(Enumerable.Repeat(chars, length)
        .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public static string GeneratePIN()
    {
        random = new Random();
        return random.Next(0, 999999).ToString("D6");
    }
}