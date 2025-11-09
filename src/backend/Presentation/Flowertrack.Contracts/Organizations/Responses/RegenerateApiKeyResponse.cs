namespace Flowertrack.Contracts.Organizations.Responses;

/// <summary>
/// Response after regenerating API key
/// </summary>
public sealed record RegenerateApiKeyResponse
{
    public string ApiKey { get; init; } = string.Empty;
    public DateTimeOffset RegeneratedAt { get; init; }
}
