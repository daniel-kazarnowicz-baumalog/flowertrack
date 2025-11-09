namespace Flowertrack.Application.Tickets.Queries.GetTicket;

/// <summary>
/// Detailed ticket data transfer object with related entities
/// </summary>
public sealed record TicketDetailDto
{
    public Guid Id { get; init; }
    public string TicketNumber { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;

    // Organization details
    public Guid OrganizationId { get; init; }
    public string OrganizationName { get; init; } = string.Empty;

    // Machine details
    public Guid MachineId { get; init; }
    public string MachineSerialNumber { get; init; } = string.Empty;
    public string MachineBrand { get; init; } = string.Empty;
    public string MachineModel { get; init; } = string.Empty;

    // User details
    public Guid CreatedByUserId { get; init; }
    public string CreatedByUserName { get; init; } = string.Empty;
    public Guid? AssignedToUserId { get; init; }
    public string? AssignedToUserName { get; init; }

    // Timestamps
    public DateTimeOffset? ResolvedAt { get; init; }
    public DateTimeOffset? ClosedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public Guid? UpdatedBy { get; init; }
}
