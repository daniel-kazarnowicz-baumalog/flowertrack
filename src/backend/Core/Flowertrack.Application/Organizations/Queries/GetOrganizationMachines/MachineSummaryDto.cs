namespace Flowertrack.Application.Organizations.Queries.GetOrganizationMachines;

/// <summary>
/// DTO for machine summary
/// </summary>
public sealed record MachineSummaryDto
{
    public Guid Id { get; init; }
    public string SerialNumber { get; init; } = string.Empty;
    public string Brand { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateOnly? LastMaintenanceDate { get; init; }
    public DateOnly? NextMaintenanceDate { get; init; }
    public DateTimeOffset RegisteredAt { get; init; }
}
