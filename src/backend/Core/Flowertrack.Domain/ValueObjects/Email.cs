using System.Text.RegularExpressions;
using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Value Object representing an email address with RFC 5322 compliant validation (simplified).
/// Email addresses are normalized to lowercase.
/// </summary>
public sealed class Email : ValueObject
{
    private const string Pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
    private static readonly Regex ValidationRegex = new(Pattern, RegexOptions.Compiled);

    private const int MaxLength = 255;

    public string Value { get; }

    private Email(string value)
    {
        var normalized = Normalize(value);
        Validate(normalized);
        Value = normalized;
    }

    /// <summary>
    /// Creates a new Email instance from the specified email address.
    /// </summary>
    /// <param name="value">The email address string.</param>
    /// <returns>A new Email instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the email format is invalid.</exception>
    public static Email Create(string value)
    {
        return new Email(value);
    }

    /// <summary>
    /// Parses an email address string into an Email instance.
    /// </summary>
    /// <param name="value">The email address string to parse.</param>
    /// <returns>An Email instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid.</exception>
    public static Email Parse(string value)
    {
        return new Email(value);
    }

    /// <summary>
    /// Tries to parse an email address string into an Email instance.
    /// </summary>
    /// <param name="value">The email address string to parse.</param>
    /// <param name="result">The resulting Email if parsing succeeds, null otherwise.</param>
    /// <returns>True if parsing succeeds, false otherwise.</returns>
    public static bool TryParse(string? value, out Email? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            var normalized = Normalize(value);

            if (normalized.Length > MaxLength)
            {
                return false;
            }

            if (!ValidationRegex.IsMatch(normalized))
            {
                return false;
            }

            result = new Email(value);
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
    /// Implicit conversion from Email to string.
    /// </summary>
    public static implicit operator string(Email email) => email.Value;

    /// <summary>
    /// Explicit conversion from string to Email.
    /// </summary>
    public static explicit operator Email(string value) => Parse(value);

    private static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Email cannot be null or empty.", nameof(value));
        }

        var trimmed = value.Trim().ToLowerInvariant();

        // Check length before further processing
        if (trimmed.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Email cannot exceed {MaxLength} characters. Got: {trimmed.Length}",
                nameof(value));
        }

        return trimmed;
    }

    private static void Validate(string normalizedEmail)
    {
        if (normalizedEmail.Length > MaxLength)
        {
            throw new ArgumentException(
                $"Email cannot exceed {MaxLength} characters. Got: {normalizedEmail.Length}",
                nameof(normalizedEmail));
        }

        if (!ValidationRegex.IsMatch(normalizedEmail))
        {
            throw new ArgumentException(
                $"Invalid email format. Got: {normalizedEmail}",
                nameof(normalizedEmail));
        }

        // Additional validation: no consecutive dots
        if (normalizedEmail.Contains(".."))
        {
            throw new ArgumentException(
                $"Invalid email format. Got: {normalizedEmail}",
                nameof(normalizedEmail));
        }
    }
}
