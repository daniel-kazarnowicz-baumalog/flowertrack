using System.Text.RegularExpressions;

namespace Flowertrack.Api.Domain.ValueObjects;

/// <summary>
/// Value object representing an email address
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase,
        TimeSpan.FromMilliseconds(250));

    /// <summary>
    /// Gets the email address value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Private constructor to enforce factory method usage
    /// </summary>
    /// <param name="value">The email address</param>
    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Email value object
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <returns>A new Email instance</returns>
    /// <exception cref="ArgumentException">Thrown when email format is invalid</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        if (email.Length > 255)
        {
            throw new ArgumentException("Email cannot exceed 255 characters", nameof(email));
        }

        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException("Invalid email format", nameof(email));
        }

        return new Email(email.ToLowerInvariant());
    }

    /// <summary>
    /// Tries to create a new Email value object
    /// </summary>
    /// <param name="email">The email address string</param>
    /// <param name="result">The created Email instance if successful</param>
    /// <returns>True if the email is valid; otherwise, false</returns>
    public static bool TryCreate(string email, out Email? result)
    {
        try
        {
            result = Create(email);
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Email email && Equals(email);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(Email email) => email.Value;

    public static bool operator ==(Email? left, Email? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Email? left, Email? right)
    {
        return !Equals(left, right);
    }
}
