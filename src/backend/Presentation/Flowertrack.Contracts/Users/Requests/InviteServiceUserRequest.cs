using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Users.Requests;

/// <summary>
/// Request to invite a new service user
/// </summary>
public sealed record InviteServiceUserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; init; } = string.Empty;

    [StringLength(50, ErrorMessage = "Phone number cannot exceed 50 characters")]
    public string? PhoneNumber { get; init; }

    [StringLength(100, ErrorMessage = "Specialization cannot exceed 100 characters")]
    public string? Specialization { get; init; }
}
