using Flowertrack.Domain.Enums;

namespace Flowertrack.Application.Tickets.Queries.GetTickets;

/// <summary>
/// DTO for ticket list item (lightweight version for lists)
/// </summary>
public sealed record TicketListItemDto
{
    /// <summary>
    /// Ticket ID
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Ticket number (e.g., TICK-2025-00001)
    /// </summary>
    public string TicketNumber { get; init; } = string.Empty;

    /// <summary>
    /// Ticket title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public TicketStatus Status { get; init; }

    /// <summary>
    /// Priority level
    /// </summary>
    public Priority Priority { get; init; }

    /// <summary>
    /// Organization ID
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization name
    /// </summary>
    public string OrganizationName { get; init; } = string.Empty;

    /// <summary>
    /// Machine ID
    /// </summary>
    public Guid? MachineId { get; init; }

    /// <summary>
    /// Machine serial number
    /// </summary>
    public string? MachineSerialNumber { get; init; }

    /// <summary>
    /// Assigned user ID (if assigned)
    /// </summary>
    public Guid? AssignedToUserId { get; init; }

    /// <summary>
    /// Assigned user full name (if assigned)
    /// </summary>
    public string? AssignedToUserName { get; init; }

    /// <summary>
    /// Created by user ID
    /// </summary>
    public Guid CreatedByUserId { get; init; }

    /// <summary>
    /// Created by user full name
    /// </summary>
    public string CreatedByUserName { get; init; } = string.Empty;

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Resolved timestamp (if resolved)
    /// </summary>
    public DateTimeOffset? ResolvedAt { get; init; }

    /// <summary>
    /// Closed timestamp (if closed)
    /// </summary>
    public DateTimeOffset? ClosedAt { get; init; }
}
