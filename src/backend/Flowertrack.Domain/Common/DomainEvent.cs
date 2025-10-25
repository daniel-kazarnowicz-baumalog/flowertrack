namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class DomainEvent
{
    /// <summary>
    /// Unique identifier of the event
    /// </summary>
    public Guid EventId { get; } = Guid.NewGuid();

    /// <summary>
    /// Date and time when the event occurred
    /// </summary>
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
