using System.Text.RegularExpressions;
using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Value Object representing a ticket number in the format TICK-{year}-{sequential}.
/// Example: TICK-2025-00001
/// </summary>
public sealed class TicketNumber : ValueObject
{
    private const string Pattern = @"^TICK-\d{4}-\d{5}$";
    private static readonly Regex ValidationRegex = new(Pattern, RegexOptions.Compiled);

    private const int MinYear = 2020;
    private const int MinSequential = 1;
    private const int MaxSequential = 99999;

    public int Year { get; }
    public int Sequential { get; }
    public string Value { get; }

    private TicketNumber(int year, int sequential)
    {
        ValidateYear(year);
        ValidateSequential(sequential);

        Year = year;
        Sequential = sequential;
        Value = $"TICK-{year:D4}-{sequential:D5}";
    }

    /// <summary>
    /// Creates a new TicketNumber with the specified year and sequential number.
    /// </summary>
    /// <param name="year">Year (must be >= 2020 and <= current year + 1)</param>
    /// <param name="sequential">Sequential number (must be > 0 and < 100000)</param>
    /// <returns>A new TicketNumber instance.</returns>
    /// <exception cref="ArgumentException">Thrown when year or sequential are invalid.</exception>
    public static TicketNumber Create(int year, int sequential)
    {
        return new TicketNumber(year, sequential);
    }

    /// <summary>
    /// Parses a ticket number string into a TicketNumber instance.
    /// </summary>
    /// <param name="value">The ticket number string to parse.</param>
    /// <returns>A TicketNumber instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid.</exception>
    public static TicketNumber Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Ticket number cannot be null or empty.", nameof(value));
        }

        if (!ValidationRegex.IsMatch(value))
        {
            throw new ArgumentException($"Invalid ticket number format. Expected format: TICK-YYYY-NNNNN. Got: {value}", nameof(value));
        }

        var parts = value.Split('-');
        var year = int.Parse(parts[1]);
        var sequential = int.Parse(parts[2]);

        return new TicketNumber(year, sequential);
    }

    /// <summary>
    /// Tries to parse a ticket number string into a TicketNumber instance.
    /// </summary>
    /// <param name="value">The ticket number string to parse.</param>
    /// <param name="result">The resulting TicketNumber if parsing succeeds, null otherwise.</param>
    /// <returns>True if parsing succeeds, false otherwise.</returns>
    public static bool TryParse(string? value, out TicketNumber? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (!ValidationRegex.IsMatch(value))
        {
            return false;
        }

        try
        {
            var parts = value.Split('-');
            var year = int.Parse(parts[1]);
            var sequential = int.Parse(parts[2]);

            result = new TicketNumber(year, sequential);
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
        yield return Year;
        yield return Sequential;
    }

    /// <summary>
    /// Implicit conversion from TicketNumber to string.
    /// </summary>
    public static implicit operator string(TicketNumber ticketNumber) => ticketNumber.Value;

    /// <summary>
    /// Explicit conversion from string to TicketNumber.
    /// </summary>
    public static explicit operator TicketNumber(string value) => Parse(value);

    private static void ValidateYear(int year)
    {
        var maxYear = DateTime.UtcNow.Year + 1;

        if (year < MinYear || year > maxYear)
        {
            throw new ArgumentException(
                $"Year must be between {MinYear} and {maxYear}. Got: {year}",
                nameof(year));
        }
    }

    private static void ValidateSequential(int sequential)
    {
        if (sequential < MinSequential || sequential > MaxSequential)
        {
            throw new ArgumentException(
                $"Sequential must be between {MinSequential} and {MaxSequential}. Got: {sequential}",
                nameof(sequential));
        }
    }
}
