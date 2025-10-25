namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Value object representing a ticket number in the format TICK-YYYY-XXXXX.
/// </summary>
public sealed class TicketNumber : IEquatable<TicketNumber>
{
    /// <summary>
    /// Gets the year component of the ticket number.
    /// </summary>
    public int Year { get; }

    /// <summary>
    /// Gets the sequential number component.
    /// </summary>
    public int SequentialNumber { get; }

    /// <summary>
    /// Gets the full ticket number string.
    /// </summary>
    public string Value => $"TICK-{Year}-{SequentialNumber:D5}";

    private TicketNumber(int year, int sequentialNumber)
    {
        Year = year;
        SequentialNumber = sequentialNumber;
    }

    /// <summary>
    /// Creates a new ticket number.
    /// </summary>
    /// <param name="year">The year component (e.g., 2025).</param>
    /// <param name="sequentialNumber">The sequential number (1-99999).</param>
    /// <returns>A new TicketNumber instance.</returns>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid.</exception>
    public static TicketNumber Create(int year, int sequentialNumber)
    {
        if (year < 2000 || year > 9999)
        {
            throw new ArgumentException("Year must be between 2000 and 9999.", nameof(year));
        }

        if (sequentialNumber < 1 || sequentialNumber > 99999)
        {
            throw new ArgumentException("Sequential number must be between 1 and 99999.", nameof(sequentialNumber));
        }

        return new TicketNumber(year, sequentialNumber);
    }

    /// <summary>
    /// Parses a ticket number string in the format TICK-YYYY-XXXXX.
    /// </summary>
    /// <param name="value">The ticket number string to parse.</param>
    /// <returns>A TicketNumber instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid.</exception>
    public static TicketNumber Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Ticket number cannot be null or whitespace.", nameof(value));
        }

        var parts = value.Split('-');
        if (parts.Length != 3 || parts[0] != "TICK")
        {
            throw new ArgumentException($"Invalid ticket number format: {value}. Expected format: TICK-YYYY-XXXXX", nameof(value));
        }

        if (!int.TryParse(parts[1], out var year))
        {
            throw new ArgumentException($"Invalid year in ticket number: {parts[1]}", nameof(value));
        }

        if (!int.TryParse(parts[2], out var sequentialNumber))
        {
            throw new ArgumentException($"Invalid sequential number in ticket number: {parts[2]}", nameof(value));
        }

        return Create(year, sequentialNumber);
    }

    public bool Equals(TicketNumber? other)
    {
        if (other is null) return false;
        return Year == other.Year && SequentialNumber == other.SequentialNumber;
    }

    public override bool Equals(object? obj) => obj is TicketNumber other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Year, SequentialNumber);

    public override string ToString() => Value;

    public static bool operator ==(TicketNumber? left, TicketNumber? right) => 
        left?.Equals(right) ?? right is null;

    public static bool operator !=(TicketNumber? left, TicketNumber? right) => 
        !(left == right);
}
