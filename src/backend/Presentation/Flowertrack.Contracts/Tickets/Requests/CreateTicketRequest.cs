using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to create a new service ticket
/// </summary>
public sealed record CreateTicketRequest
{
    /// <summary>
    /// Organization ID for which the ticket is created
    /// </summary>
    [Required(ErrorMessage = "Organization ID is required")]
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Machine ID related to the ticket
    /// </summary>
    [Required(ErrorMessage = "Machine ID is required")]
    public Guid MachineId { get; init; }

    /// <summary>
    /// Ticket title
    /// </summary>
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Detailed description of the issue
    /// </summary>
    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Ticket priority level (0=Low, 1=Medium, 2=High, 3=Critical)
    /// </summary>
    [Required(ErrorMessage = "Priority is required")]
    [Range(0, 3, ErrorMessage = "Priority must be between 0 (Low) and 3 (Critical)")]
    public int Priority { get; init; }
}
