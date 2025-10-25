using Flowertrack.Domain.Enums;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Placeholder entity for Machine aggregate root.
/// Full implementation will be provided in Phase 1.1 (Issue #2).
/// </summary>
public class Machine
{
    public Guid Id { get; set; }
    public Guid OrganizationId { get; set; }
    public string SerialNumber { get; set; } = null!;
    public string? Brand { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
    public MachineStatus Status { get; set; }
    public string? ApiToken { get; set; }
    public DateOnly? LastMaintenanceDate { get; set; }
    public DateOnly? NextMaintenanceDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
