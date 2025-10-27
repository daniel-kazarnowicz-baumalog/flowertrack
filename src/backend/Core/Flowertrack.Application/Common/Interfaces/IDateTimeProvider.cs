namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for providing testable date/time functionality
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time
    /// </summary>
    DateTimeOffset UtcNow { get; }

    /// <summary>
    /// Gets the current UTC date
    /// </summary>
    DateOnly Today { get; }
}
