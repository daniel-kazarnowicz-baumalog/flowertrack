namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Represents a maintenance interval configuration
/// </summary>
public sealed class MaintenanceInterval
{
    /// <summary>
    /// Unique identifier for the maintenance interval
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Number of days between maintenance operations
    /// </summary>
    public int Days { get; private set; }

    /// <summary>
    /// Description of the maintenance interval
    /// </summary>
    public string Description { get; private set; }

    private MaintenanceInterval(int id, int days, string description)
    {
        if (days <= 0)
            throw new ArgumentException("Maintenance interval must be greater than zero", nameof(days));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        Id = id;
        Days = days;
        Description = description;
    }

    /// <summary>
    /// Create a new maintenance interval
    /// </summary>
    public static MaintenanceInterval Create(int id, int days, string description)
    {
        return new MaintenanceInterval(id, days, description);
    }

    /// <summary>
    /// Calculate the next maintenance date from a given date
    /// </summary>
    public DateOnly CalculateNextDate(DateOnly fromDate)
    {
        return fromDate.AddDays(Days);
    }
}
