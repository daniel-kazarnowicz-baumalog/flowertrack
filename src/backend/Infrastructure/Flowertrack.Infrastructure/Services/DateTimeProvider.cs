using Flowertrack.Application.Common.Interfaces;

namespace Flowertrack.Infrastructure.Services;

/// <summary>
/// Service for providing testable date/time functionality
/// </summary>
public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}
