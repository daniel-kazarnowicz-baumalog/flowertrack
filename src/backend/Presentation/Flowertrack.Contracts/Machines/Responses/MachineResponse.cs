namespace Flowertrack.Contracts.Machines.Responses;

/// <summary>
/// Response for machine
/// </summary>
public sealed record MachineResponse
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Location { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}
