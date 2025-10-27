using System.ComponentModel.DataAnnotations;

namespace Flowertrack.Contracts.Machines.Requests;

/// <summary>
/// Request to register a new machine
/// </summary>
public sealed record RegisterMachineRequest
{
    [Required(ErrorMessage = "Organization ID is required")]
    public Guid OrganizationId { get; init; }

    [Required(ErrorMessage = "Serial number is required")]
    [StringLength(255, ErrorMessage = "Serial number cannot exceed 255 characters")]
    public string SerialNumber { get; init; } = string.Empty;

    [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
    public string? Brand { get; init; }

    [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
    public string? Model { get; init; }

    [StringLength(255, ErrorMessage = "Location cannot exceed 255 characters")]
    public string? Location { get; init; }
}
