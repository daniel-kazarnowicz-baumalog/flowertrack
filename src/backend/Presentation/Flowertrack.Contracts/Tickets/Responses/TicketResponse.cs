namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Comprehensive ticket response with full details
/// Used for single ticket retrieval and detailed views
/// </summary>
public sealed record TicketResponse
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Unique ticket number (e.g., TICK-2025-00001)
    /// </summary>
    public string TicketNumber { get; init; } = string.Empty;

    /// <summary>
    /// Ticket title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Detailed description
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Current status
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Priority level
    /// </summary>
    public string Priority { get; init; } = string.Empty;

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
    public Guid MachineId { get; init; }

    /// <summary>
    /// Machine serial number
    /// </summary>
    public string MachineSerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// Machine model information
    /// </summary>
    public string MachineModel { get; init; } = string.Empty;

    /// <summary>
    /// User ID who created the ticket
    /// </summary>
    public Guid CreatedByUserId { get; init; }

    /// <summary>
    /// Name of user who created the ticket
    /// </summary>
    public string CreatedByUserName { get; init; } = string.Empty;

    /// <summary>
    /// User ID assigned to the ticket (if any)
    /// </summary>
    public Guid? AssignedToUserId { get; init; }

    /// <summary>
    /// Name of assigned user (if any)
    /// </summary>
    public string? AssignedToUserName { get; init; }

    /// <summary>
    /// When ticket was resolved (if applicable)
    /// </summary>
    public DateTimeOffset? ResolvedAt { get; init; }

    /// <summary>
    /// When ticket was closed (if applicable)
    /// </summary>
    public DateTimeOffset? ClosedAt { get; init; }

    /// <summary>
    /// When ticket was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When ticket was last updated
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// User who last updated the ticket
    /// </summary>
    public Guid? UpdatedBy { get; init; }
}
