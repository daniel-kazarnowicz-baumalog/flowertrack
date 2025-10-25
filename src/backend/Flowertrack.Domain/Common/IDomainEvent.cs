namespace Flowertrack.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent significant occurrences in the domain that domain experts care about.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// The date and time when the event occurred.
    /// </summary>
    DateTimeOffset OccurredOn { get; }
}
