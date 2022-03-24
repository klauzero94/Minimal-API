using System.Security.Cryptography;
using System.Text;

namespace Utils.Security;

public class PasswordHasher
{
    public bool Compare(string password, string passwordHash, string salt)
    {
        string _passwordHash = GenerateHash(password, salt);
        return _passwordHash.Equals(passwordHash);
    }

    public string GenerateHash(string password, string salt)
    {
        byte[] _password = Encoding.ASCII.GetBytes(password);
        byte[] _salt = Encoding.ASCII.GetBytes(salt);
        var pbkdf2 = new Rfc2898DeriveBytes(_password, _salt, 100000);
        byte[] _hash = pbkdf2.GetBytes(20);
        byte[] _hashBytes = new byte[36];
        Array.Copy(_salt, 0, _hashBytes, 0, 16);
        Array.Copy(_hash, 0, _hashBytes, 16, 20);
        return Convert.ToBase64String(_hashBytes);
    }

    public string GenerateSalt()
    {
        byte[] _salt;
        RandomNumberGenerator.Create().GetBytes(_salt = new byte[16]);
        return Convert.ToBase64String(_salt);
    }
}