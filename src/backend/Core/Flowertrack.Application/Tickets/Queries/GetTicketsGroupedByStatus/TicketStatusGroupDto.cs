using Flowertrack.Domain.Enums;

namespace Flowertrack.Application.Tickets.Queries.GetTicketsGroupedByStatus;

/// <summary>
/// DTO representing a group of tickets by status
/// </summary>
public sealed record TicketStatusGroupDto
{
    /// <summary>
    /// Status of this group
    /// </summary>
    public TicketStatus Status { get; init; }

    /// <summary>
    /// Total count of tickets in this status
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// Sample tickets from this status group (limited by SampleSize)
    /// </summary>
    public List<TicketSampleDto> SampleTickets { get; init; } = new();
}

/// <summary>
/// Lightweight DTO for sample tickets in grouped view
/// </summary>
public sealed record TicketSampleDto
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
    /// Priority level
    /// </summary>
    public Priority Priority { get; init; }

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
