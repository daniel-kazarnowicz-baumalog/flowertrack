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
        OccurredOn = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc/>
    public DateTimeOffset OccurredOn { get; }
}
