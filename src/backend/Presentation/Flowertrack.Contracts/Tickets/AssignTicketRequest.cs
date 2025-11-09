using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets;

/// <summary>
/// Request to assign a ticket to a service technician
/// </summary>
public sealed record AssignTicketRequest
{
    /// <summary>
    /// User ID of the service technician to assign the ticket to
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    [Required(ErrorMessage = "AssignedToUserId is required")]
    public Guid AssignedToUserId { get; init; }
}
