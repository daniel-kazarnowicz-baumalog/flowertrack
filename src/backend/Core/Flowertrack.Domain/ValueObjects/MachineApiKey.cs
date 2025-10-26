using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Value Object representing a secure API key for machine authentication.
/// Format: mch_{base64_token}
/// </summary>
public sealed class MachineApiKey : ValueObject
{
    private const string Pattern = @"^mch_[a-zA-Z0-9-]{32,40}$";
    private static readonly Regex ValidationRegex = new(Pattern, RegexOptions.Compiled);

    private const string Prefix = "mch_";
    private const int TokenByteLength = 24; // Will generate 32 base64 characters

    public string Value { get; }

    private MachineApiKey(string value)
    {
        Validate(value);
        Value = value;
    }

    /// <summary>
    /// Generates a new secure MachineApiKey using cryptographically secure random number generation.
    /// </summary>
    /// <returns>A new MachineApiKey instance with a randomly generated token.</returns>
    public static MachineApiKey Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenByteLength);
        var base64Token = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "-")
            .Replace("=", "");

        // Ensure we have at least 32 characters
        if (base64Token.Length < 32)
        {
            // Generate more bytes if needed
            var additionalBytes = RandomNumberGenerator.GetBytes(8);
            base64Token += Convert.ToBase64String(additionalBytes)
                .Replace("+", "-")
                .Replace("/", "-")
                .Replace("=", "");
        }

        // Take exactly 32 characters for consistent length
        base64Token = base64Token[..32];

        var value = $"{Prefix}{base64Token}";
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
            if (!value.StartsWith(Prefix))
            {
                return false;
            }

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

        if (!value.StartsWith(Prefix))
        {
            throw new ArgumentException(
                $"Machine API key must start with '{Prefix}'. Got: {value}",
                nameof(value));
        }

        if (!ValidationRegex.IsMatch(value))
        {
            throw new ArgumentException(
                $"Machine API key has invalid format. Expected format: mch_{{32-40 alphanumeric characters}}. Got: {value}",
                nameof(value));
        }
    }
}
