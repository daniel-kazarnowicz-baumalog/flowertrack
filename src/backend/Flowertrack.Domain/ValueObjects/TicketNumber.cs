namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Value object representing a ticket number in the format TICK-YYYY-XXXXX
/// </summary>
public sealed class TicketNumber : IEquatable<TicketNumber>
{
    private const string Prefix = "TICK";
    private const int SequenceLength = 5;

    public string Value { get; }

    private TicketNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new ticket number for the given year and sequence
    /// </summary>
    /// <param name="year">The year (e.g., 2025)</param>
    /// <param name="sequence">The sequence number (1-99999)</param>
    /// <returns>A new TicketNumber instance</returns>
    /// <exception cref="ArgumentException">Thrown when year or sequence is invalid</exception>
    public static TicketNumber Create(int year, int sequence)
    {
        if (year < 2000 || year > 9999)
        {
            throw new ArgumentException("Year must be between 2000 and 9999", nameof(year));
        }

        if (sequence < 1 || sequence > 99999)
        {
            throw new ArgumentException("Sequence must be between 1 and 99999", nameof(sequence));
        }

        var sequenceStr = sequence.ToString().PadLeft(SequenceLength, '0');
        var value = $"{Prefix}-{year}-{sequenceStr}";

        return new TicketNumber(value);
    }

    /// <summary>
    /// Parses a ticket number from a string
    /// </summary>
    /// <param name="value">The string value to parse</param>
    /// <returns>A TicketNumber instance</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid</exception>
    public static TicketNumber Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Ticket number cannot be null or empty", nameof(value));
        }

        var parts = value.Split('-');
        if (parts.Length != 3)
        {
            throw new ArgumentException($"Invalid ticket number format: {value}. Expected format: TICK-YYYY-XXXXX", nameof(value));
        }

        if (parts[0] != Prefix)
        {
            throw new ArgumentException($"Invalid ticket number prefix: {parts[0]}. Expected: {Prefix}", nameof(value));
        }

        if (!int.TryParse(parts[1], out var year) || year < 2000 || year > 9999)
        {
            throw new ArgumentException($"Invalid year in ticket number: {parts[1]}", nameof(value));
        }

        if (!int.TryParse(parts[2], out var sequence) || parts[2].Length != SequenceLength)
        {
            throw new ArgumentException($"Invalid sequence in ticket number: {parts[2]}", nameof(value));
        }

        return new TicketNumber(value);
    }

    /// <summary>
    /// Tries to parse a ticket number from a string
    /// </summary>
    public static bool TryParse(string value, out TicketNumber? ticketNumber)
    {
        try
        {
            ticketNumber = Parse(value);
            return true;
        }
        catch
        {
            ticketNumber = null;
            return false;
        }
    }

    public bool Equals(TicketNumber? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is TicketNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static bool operator ==(TicketNumber? left, TicketNumber? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TicketNumber? left, TicketNumber? right)
    {
        return !Equals(left, right);
    }

    public static implicit operator string(TicketNumber ticketNumber)
    {
        return ticketNumber.Value;
    }
}
