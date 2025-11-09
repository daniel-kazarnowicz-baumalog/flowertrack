namespace Flowertrack.Contracts.Machines.Responses;

public sealed record RegenerateMachineTokenResponse
{
    public string ApiToken { get; init; } = string.Empty;
    public DateTimeOffset RegeneratedAt { get; init; }
}
