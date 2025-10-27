namespace Flowertrack.Contracts.Common;

/// <summary>
/// Standard error response
/// </summary>
public sealed record ErrorResponse
{
    public string Message { get; init; }
    public string? Detail { get; init; }
    public DateTimeOffset Timestamp { get; init; }

    public ErrorResponse(string message, string? detail = null)
    {
        Message = message;
        Detail = detail;
        Timestamp = DateTimeOffset.UtcNow;
    }
}
