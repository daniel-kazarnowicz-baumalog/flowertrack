using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Organizations.Requests;

/// <summary>
/// Request to onboard a new organization
/// </summary>
public sealed record OnboardOrganizationRequest
{
    [Required(ErrorMessage = "Organization name is required")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Organization name must be between 3 and 255 characters")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Admin email is required")]
    [EmailAddress(ErrorMessage = "Admin email must be a valid email address")]
    public string AdminEmail { get; init; } = string.Empty;

    [Required(ErrorMessage = "Admin first name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string AdminFirstName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Admin last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string AdminLastName { get; init; } = string.Empty;

    [StringLength(50, ErrorMessage = "Phone cannot exceed 50 characters")]
    public string? Phone { get; init; }

    [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters")]
    public string? Address { get; init; }

    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; init; }

    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string? PostalCode { get; init; }

    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; init; }

    public string? Notes { get; init; }
}
