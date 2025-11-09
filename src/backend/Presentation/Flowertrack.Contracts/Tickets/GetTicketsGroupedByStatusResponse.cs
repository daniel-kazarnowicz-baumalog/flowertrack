namespace Flowertrack.Contracts.Tickets;

/// <summary>
/// Response containing tickets grouped by status
/// </summary>
public sealed record GetTicketsGroupedByStatusResponse
{
    /// <summary>
    /// List of status groups
    /// </summary>
    public List<TicketStatusGroup> Groups { get; init; } = new();
}

/// <summary>
/// Represents a group of tickets by status
/// </summary>
public sealed record TicketStatusGroup
{
    /// <summary>
    /// Status of this group (0=New, 1=Accepted, 2=InProgress, 3=Resolved, 4=Closed, 5=Reopened)
    /// </summary>
    public int Status { get; init; }

    /// <summary>
    /// Status name
    /// </summary>
    public string StatusName { get; init; } = string.Empty;

    /// <summary>
    /// Total count of tickets in this status
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Sample tickets from this status group
    /// </summary>
    public List<TicketSample> SampleTickets { get; init; } = new();
}

/// <summary>
/// Lightweight ticket sample for grouped view
/// </summary>
public sealed record TicketSample
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
    /// Priority level (0=Low, 1=Medium, 2=High, 3=Critical)
    /// </summary>
    public int Priority { get; init; }

    /// <summary>
    /// Priority name
    /// </summary>
    public string PriorityName { get; init; } = string.Empty;

    /// <summary>
    /// Organization name
    /// </summary>
    public string OrganizationName { get; init; } = string.Empty;

    /// <summary>
    /// Machine serial number (if assigned)
    /// </summary>
    public string? MachineSerialNumber { get; init; }

    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
