using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Tickets.Requests;

/// <summary>
/// Request to update an existing ticket's basic information
/// All fields are optional - only provided fields will be updated
/// </summary>
public sealed record UpdateTicketRequest
{
    /// <summary>
    /// Updated title (optional)
    /// </summary>
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string? Title { get; init; }

    /// <summary>
    /// Updated description (optional)
    /// </summary>
    [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
    public string? Description { get; init; }

    /// <summary>
    /// Updated priority level (0=Low, 1=Medium, 2=High, 3=Critical) (optional)
    /// </summary>
    [Range(0, 3, ErrorMessage = "Priority must be between 0 (Low) and 3 (Critical)")]
    public int? Priority { get; init; }
}
