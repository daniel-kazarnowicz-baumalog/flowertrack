namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    DateTimeOffset OccurredAt { get; }
}
