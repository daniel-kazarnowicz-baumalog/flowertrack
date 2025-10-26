namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for domain events.
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
    /// Initializes a new instance of the <see cref="DomainEvent"/> class with an aggregate ID.
    /// </summary>
    /// <param name="aggregateId">The ID of the aggregate that produced this event</param>
    protected DomainEvent(Guid aggregateId) : this()
    {
        AggregateId = aggregateId;
    }

    /// <inheritdoc/>
    public Guid EventId { get; }

    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; }

    /// <inheritdoc/>
    public Guid? AggregateId { get; }
}
