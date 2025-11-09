namespace Flowertrack.Contracts.Machines.Responses;

public sealed record MachineDetailsResponse
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? Location { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? ApiToken { get; init; }
    public DateOnly? LastMaintenanceDate { get; init; }
    public DateOnly? NextMaintenanceDate { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
