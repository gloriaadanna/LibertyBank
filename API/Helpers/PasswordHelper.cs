using System.Text;

namespace API.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string password,string salt)
    {
        string Password;
        byte[] hashedPassword;
          
        using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(salt)))
        {
            hashedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        
        var passwordStringBuilder = new StringBuilder();
        foreach (var letter in hashedPassword)
        {
            passwordStringBuilder.Append(letter.ToString("x2"));
        }
        Password = passwordStringBuilder.ToString();;
        return Password;
    }
    
    public static bool VerifyPassword(string password,string storedPassword, string storedSalt)
    {
        byte[] passwordHash;
        using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(storedSalt)))
        {
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
        StringBuilder passwordStringBuilder = new StringBuilder();
        foreach (var letter in passwordHash)
        {
            passwordStringBuilder.Append(letter.ToString("x2"));
        }
        var confirmPassword = passwordStringBuilder.ToString();
        if (confirmPassword.Length == storedPassword.Length)
        {
            if (string.Equals( confirmPassword, storedPassword, StringComparison.Ordinal))
            {
                return true;
            }
        }
        return false;
    }
}