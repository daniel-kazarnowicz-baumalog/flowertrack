using System.Security.Cryptography;

namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Represents a secure API key for machine authentication
/// </summary>
public sealed class MachineApiKey
{
    private const int TokenLength = 32;
    private static readonly char[] AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();

    /// <summary>
    /// The API token value
    /// </summary>
    public string Value { get; private set; }

    /// <summary>
    /// When the token was generated
    /// </summary>
    public DateTime GeneratedAt { get; private set; }

    private MachineApiKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("API token value cannot be empty", nameof(value));

        if (value.Length < TokenLength)
            throw new ArgumentException($"API token must be at least {TokenLength} characters", nameof(value));

        Value = value;
        GeneratedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Generate a new secure API token
    /// </summary>
    public static MachineApiKey Generate()
    {
        var tokenBytes = new byte[TokenLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);

        // Convert to base62-like string using allowed characters
        var token = new char[TokenLength];
        for (int i = 0; i < TokenLength; i++)
        {
            token[i] = AllowedChars[tokenBytes[i] % AllowedChars.Length];
        }

        return new MachineApiKey(new string(token));
    }

    /// <summary>
    /// Create an API key from an existing token value (for reconstitution from database)
    /// </summary>
    public static MachineApiKey FromValue(string value)
    {
        return new MachineApiKey(value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not MachineApiKey other)
            return false;

        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        // Only show first 8 characters for security
        return Value.Length > 8 ? $"{Value[..8]}..." : Value;
    }

    public static bool operator ==(MachineApiKey? left, MachineApiKey? right)
    {
        if (left is null && right is null)
            return true;
        if (left is null || right is null)
            return false;
        return left.Equals(right);
    }

    public static bool operator !=(MachineApiKey? left, MachineApiKey? right)
    {
        return !(left == right);
    }
}
