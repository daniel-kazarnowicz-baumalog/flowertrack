using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Organizations.Requests;

/// <summary>
/// Request to update organization information
/// </summary>
public sealed record UpdateOrganizationRequest
{
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Organization name must be between 3 and 255 characters")]
    public string? Name { get; init; }

    [EmailAddress(ErrorMessage = "Email must be a valid email address")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; init; }

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
