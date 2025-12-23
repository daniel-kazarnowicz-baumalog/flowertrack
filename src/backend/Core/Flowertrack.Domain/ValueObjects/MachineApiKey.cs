using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Value Object representing a secure API key for machine authentication.
/// Format: {12_random_alphanumeric_chars}
/// Maximum 12 characters, unique, permanently assigned to machine
/// </summary>
public sealed class MachineApiKey : ValueObject
{
    private const string Pattern = @"^[A-Za-z0-9]{12}$";
    private static readonly Regex ValidationRegex = new(Pattern, RegexOptions.Compiled);

    private const int TokenLength = 12; // Exactly 12 characters
    private const string AllowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789"; // Removed confusing chars: 0, O, 1, I, l

    public string Value { get; }

    private MachineApiKey(string value)
    {
        Validate(value);
        Value = value;
    }

    /// <summary>
    /// Generates a new secure MachineApiKey using cryptographically secure random number generation.
    /// Generates exactly 12 random alphanumeric characters (avoiding confusing characters like 0, O, 1, I, l)
    /// </summary>
    /// <returns>A new MachineApiKey instance with a randomly generated token.</returns>
    public static MachineApiKey Generate()
    {
        var result = new char[TokenLength];
        var randomBytes = RandomNumberGenerator.GetBytes(TokenLength);
        
        for (int i = 0; i < TokenLength; i++)
        {
            result[i] = AllowedChars[randomBytes[i] % AllowedChars.Length];
        }
        
        var value = new string(result);
        return new MachineApiKey(value);
    }

    /// <summary>
    /// Creates a MachineApiKey from an existing token string.
    /// </summary>
    /// <param name="value">The API key token string.</param>
    /// <returns>A MachineApiKey instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid.</exception>
    public static MachineApiKey Create(string value)
    {
        return new MachineApiKey(value);
    }

    /// <summary>
    /// Tries to create a MachineApiKey from an existing token string.
    /// </summary>
    /// <param name="value">The API key token string.</param>
    /// <param name="result">The resulting MachineApiKey if creation succeeds, null otherwise.</param>
    /// <returns>True if creation succeeds, false otherwise.</returns>
    public static bool TryCreate(string? value, out MachineApiKey? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            if (!ValidationRegex.IsMatch(value))
            {
                return false;
            }

            result = new MachineApiKey(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <summary>
    /// Implicit conversion from MachineApiKey to string.
    /// </summary>
    public static implicit operator string(MachineApiKey apiKey) => apiKey.Value;

    /// <summary>
    /// Explicit conversion from string to MachineApiKey.
    /// </summary>
    public static explicit operator MachineApiKey(string value) => Create(value);

    private static void Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Machine API key cannot be null or empty.", nameof(value));
        }

        if (!ValidationRegex.IsMatch(value))
        {
            throw new ArgumentException(
                $"Machine API key has invalid format. Expected format: exactly 12 alphanumeric characters. Got: {value}",
                nameof(value));
        }
    }
}
