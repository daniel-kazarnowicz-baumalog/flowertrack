namespace Flowertrack.Contracts.Machines.Requests;

public sealed record MachineAlarmRequest
{
    public string Reason { get; init; } = string.Empty;
}
