namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    /// <summary>
    /// Date and time when the event occurred (UTC)
    /// </summary>
    public DateTime OccurredOn { get; }

    protected DomainEventBase()
    {
        OccurredOn = DateTime.UtcNow;
    }
}
