namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }

    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
