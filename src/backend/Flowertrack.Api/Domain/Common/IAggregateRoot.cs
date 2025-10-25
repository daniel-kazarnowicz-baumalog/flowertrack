namespace Flowertrack.Api.Domain.Common;

/// <summary>
/// Marker interface for aggregate roots in DDD
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Get all domain events raised by this aggregate
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clear all domain events after they have been published
    /// </summary>
    void ClearDomainEvents();
}
