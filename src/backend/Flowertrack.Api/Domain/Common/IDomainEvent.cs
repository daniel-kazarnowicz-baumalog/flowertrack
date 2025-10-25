namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Marker interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Date and time when the event occurred (UTC)
    /// </summary>
    DateTime OccurredOn { get; }
}
