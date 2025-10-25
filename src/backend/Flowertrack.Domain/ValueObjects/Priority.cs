namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Ticket priority level
/// </summary>
public enum Priority
{
    /// <summary>
    /// Low priority - can wait
    /// </summary>
    Low = 1,

    /// <summary>
    /// Medium priority - normal processing
    /// </summary>
    Medium = 2,

    /// <summary>
    /// High priority - urgent
    /// </summary>
    High = 3,

    /// <summary>
    /// Critical priority - immediate attention required
    /// </summary>
    Critical = 4
}
