namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class.
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Unique identifier for this event instance
    /// </summary>
    public Guid EventId { get; }

    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; }
}
