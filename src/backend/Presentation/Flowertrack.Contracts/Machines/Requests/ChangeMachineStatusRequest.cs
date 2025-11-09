namespace Flowertrack.Contracts.Machines.Requests;

public sealed record ChangeMachineStatusRequest
{
    public string NewStatus { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}
