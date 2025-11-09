namespace Flowertrack.Contracts.Organizations.Responses;

/// <summary>
/// Response for organization details
/// </summary>
public sealed record OrganizationDetailsResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? City { get; init; }
    public string? PostalCode { get; init; }
    public string? Country { get; init; }
    public string ServiceStatus { get; init; } = string.Empty;
    public DateTimeOffset? ContractStartDate { get; init; }
    public DateTimeOffset? ContractEndDate { get; init; }
    public string? Notes { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
