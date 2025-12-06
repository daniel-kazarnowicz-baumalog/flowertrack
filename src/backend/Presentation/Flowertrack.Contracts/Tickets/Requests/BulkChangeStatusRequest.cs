using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to change status of multiple tickets
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkChangeStatusRequest
{
    /// <summary>
    /// List of ticket IDs to update
    /// </summary>
    /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6"]</example>
    [Required(ErrorMessage = "TicketIds is required")]
    [MinLength(1, ErrorMessage = "At least one ticket ID is required")]
    [MaxLength(100, ErrorMessage = "Cannot process more than 100 tickets at once")]
    public IReadOnlyList<Guid> TicketIds { get; init; } = [];

    /// <summary>
    /// New status to apply to all tickets (0=New, 1=Accepted, 2=InProgress, 3=Resolved, 4=Closed, 5=Reopened)
    /// </summary>
    /// <example>3</example>
    [Required(ErrorMessage = "Status is required")]
    [Range(0, 5, ErrorMessage = "Status must be between 0 and 5")]
    public int Status { get; init; }

    /// <summary>
    /// Reason for status change (required for Resolved/Closed, minimum 10 characters)
    /// </summary>
    /// <example>Ticket has been resolved</example>
    [MaxLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
    public string? Reason { get; init; }
}
