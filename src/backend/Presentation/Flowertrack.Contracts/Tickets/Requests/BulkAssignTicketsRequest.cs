using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to assign multiple tickets to a service technician
/// US-014: Masowe akcje na zgłoszeniach
/// </summary>
public sealed record BulkAssignTicketsRequest
{
    /// <summary>
    /// List of ticket IDs to assign
    /// </summary>
    /// <example>["3fa85f64-5717-4562-b3fc-2c963f66afa6"]</example>
    [Required(ErrorMessage = "TicketIds is required")]
    [MinLength(1, ErrorMessage = "At least one ticket ID is required")]
    [MaxLength(100, ErrorMessage = "Cannot process more than 100 tickets at once")]
    public IReadOnlyList<Guid> TicketIds { get; init; } = [];

    /// <summary>
    /// User ID of the service technician to assign tickets to
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required(ErrorMessage = "AssignToUserId is required")]
    public Guid AssignToUserId { get; init; }
}
