using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to archive multiple tickets
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkArchiveTicketsRequest
{
    /// <summary>
    /// List of ticket IDs to archive
    /// </summary>
    /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6"]</example>
    [Required(ErrorMessage = "TicketIds is required")]
    [MinLength(1, ErrorMessage = "At least one ticket ID is required")]
    [MaxLength(100, ErrorMessage = "Cannot process more than 100 tickets at once")]
    public IReadOnlyList<Guid> TicketIds { get; init; } = [];

    /// <summary>
    /// Reason for archiving
    /// </summary>
    /// <example>Bulk archive of old tickets</example>
    [MaxLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
    public string? Reason { get; init; }
}
