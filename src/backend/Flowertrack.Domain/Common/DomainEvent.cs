namespace Flowertrack.Domain.Common;

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.UtcNow;
}
