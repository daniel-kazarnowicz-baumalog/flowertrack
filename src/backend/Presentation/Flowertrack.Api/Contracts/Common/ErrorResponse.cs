namespace Flowertrack.Api.Contracts.Common;

/// <summary>
/// RFC 7807 compliant error response model
/// </summary>
public record ErrorResponse
{
    /// <summary>
    /// A URI reference that identifies the problem type
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// A short, human-readable summary of the problem type
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// The HTTP status code
    /// </summary>
    public required int Status { get; init; }

    /// <summary>
    /// A human-readable explanation specific to this occurrence of the problem
    /// </summary>
    public string? Detail { get; init; }

    /// <summary>
    /// A URI reference that identifies the specific occurrence of the problem
    /// </summary>
    public string? Instance { get; init; }

    /// <summary>
    /// Additional error details (e.g., validation errors)
    /// </summary>
    public Dictionary<string, object>? Extensions { get; init; }
}
