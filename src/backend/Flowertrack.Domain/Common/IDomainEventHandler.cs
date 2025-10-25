namespace Flowertrack.Domain.Common;

/// <summary>
/// Interface for handling domain events.
/// Implementations of this interface will be invoked when a domain event is published.
/// </summary>
/// <typeparam name="TEvent">The type of domain event to handle</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}
