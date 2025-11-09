using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets;

/// <summary>
/// Request to update a ticket's status
/// </summary>
public sealed record UpdateTicketStatusRequest
{
    /// <summary>
    /// New status for the ticket (0=New, 1=Accepted, 2=InProgress, 3=Resolved, 4=Closed, 5=Reopened)
    /// </summary>
    /// <example>2</example>
    [Required(ErrorMessage = "Status is required")]
    [Range(0, 5, ErrorMessage = "Status must be between 0 (New) and 5 (Reopened)")]
    public int Status { get; init; }

    /// <summary>
    /// Reason for the status change (required for Resolved and Closed statuses)
    /// </summary>
    /// <example>Customer confirmed the issue is fixed</example>
    [StringLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
    public string? Reason { get; init; }
}
