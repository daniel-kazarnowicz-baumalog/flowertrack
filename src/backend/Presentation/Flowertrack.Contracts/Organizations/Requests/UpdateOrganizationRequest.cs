namespace Flowertrack.Contracts.Organizations.Requests;

/// <summary>
/// Request to update organization information
/// </summary>
public sealed record UpdateOrganizationRequest
{
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string? Notes { get; init; }
}
