using Flowertrack.Domain.Enums;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Placeholder entity for Ticket aggregate root.
/// Full implementation will be provided in Phase 1.1 (Issue #1).
/// </summary>
public class Ticket
{
    public Guid Id { get; set; }
    public TicketNumber TicketNumber { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public Guid OrganizationId { get; set; }
    public Guid MachineId { get; set; }
    public TicketStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTimeOffset? ResolvedAt { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
