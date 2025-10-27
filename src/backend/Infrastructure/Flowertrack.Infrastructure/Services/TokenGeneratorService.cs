using System.Security.Cryptography;
using Flowertrack.Application.Common.Interfaces;

namespace Flowertrack.Infrastructure.Services;

/// <summary>
/// Service for generating secure tokens
/// </summary>
public sealed class TokenGeneratorService : ITokenGenerator
{
    private const string PasswordChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";

    public string GenerateSecureToken(int length = 32)
    {
        var randomBytes = new byte[length];
        RandomNumberGenerator.Fill(randomBytes);

        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }

    public string GenerateTemporaryPassword(int length = 12)
    {
        var password = new char[length];

        for (int i = 0; i < length; i++)
        {
            password[i] = PasswordChars[RandomNumberGenerator.GetInt32(PasswordChars.Length)];
        }

        // Ensure password contains at least one uppercase, lowercase, digit, and special char
        password[0] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"[RandomNumberGenerator.GetInt32(26)];
        password[1] = "abcdefghijklmnopqrstuvwxyz"[RandomNumberGenerator.GetInt32(26)];
        password[2] = "0123456789"[RandomNumberGenerator.GetInt32(10)];
        password[3] = "!@#$%^&*"[RandomNumberGenerator.GetInt32(8)];

        // Shuffle the password
        for (int i = length - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }
}
