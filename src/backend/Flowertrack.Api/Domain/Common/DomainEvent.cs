namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base class for domain events
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    public DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class
    /// </summary>
    protected DomainEvent()
    {
        OccurredAt = DateTimeOffset.UtcNow;
    }
}
