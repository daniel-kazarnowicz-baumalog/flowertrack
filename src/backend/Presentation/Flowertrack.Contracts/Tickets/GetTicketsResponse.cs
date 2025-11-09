namespace Flowertrack.Contracts.Tickets;

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
    /// Current status (0=New, 1=Accepted, 2=InProgress, 3=Resolved, 4=Closed, 5=Reopened)
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Priority level (0=Low, 1=Medium, 2=High, 3=Critical)
    /// </summary>
    public int Priority { get; init; }

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

/// <summary>
/// Response containing paginated list of tickets
/// </summary>
public sealed record GetTicketsResponse
{
    /// <summary>
    /// List of tickets for the current page
    /// </summary>
    public IReadOnlyList<TicketListItemDto> Items { get; init; } = Array.Empty<TicketListItemDto>();

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages { get; init; }

    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPreviousPage { get; init; }

    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNextPage { get; init; }
}
