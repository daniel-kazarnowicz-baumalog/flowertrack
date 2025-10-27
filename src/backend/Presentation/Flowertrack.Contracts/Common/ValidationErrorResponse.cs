namespace Flowertrack.Contracts.Common;

/// <summary>
/// Validation error response
/// </summary>
public sealed record ValidationErrorResponse
{
    public string Message { get; init; } = "One or more validation errors occurred";
    public Dictionary<string, string[]> Errors { get; init; } = new();
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
}
