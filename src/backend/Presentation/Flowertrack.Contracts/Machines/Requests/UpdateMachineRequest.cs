namespace Flowertrack.Contracts.Machines.Requests;

public sealed record UpdateMachineRequest
{
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Location { get; init; }
}
