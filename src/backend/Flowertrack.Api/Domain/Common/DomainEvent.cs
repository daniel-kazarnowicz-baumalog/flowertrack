namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for all domain events in the system.
/// Domain events represent something that has happened in the domain that domain experts care about.
/// </summary>
public abstract record DomainEvent
{
    /// <summary>
    /// Unique identifier for this event instance
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// When this event occurred
    /// </summary>
    public DateTimeOffset OccurredOn { get; init; }

    /// <summary>
    /// Optional identifier of the aggregate that produced this event
    /// </summary>
    public Guid? AggregateId { get; init; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTimeOffset.UtcNow;
    }

    protected DomainEvent(Guid aggregateId) : this()
    {
        AggregateId = aggregateId;
    }
}
