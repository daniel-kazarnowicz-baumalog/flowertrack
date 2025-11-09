namespace Flowertrack.Application.Machines.Queries.GetMachine;

public sealed record MachineDetailsDto
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
    public int? MaintenanceIntervalId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public Guid? CreatedBy { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
}
