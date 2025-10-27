namespace Flowertrack.Contracts.Organizations.Responses;

/// <summary>
/// Response for organization in list view
/// </summary>
public sealed record OrganizationResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? City { get; init; }
    public string? Country { get; init; }
    public string ServiceStatus { get; init; } = string.Empty;
    public DateTimeOffset? ContractStartDate { get; init; }
    public DateTimeOffset? ContractEndDate { get; init; }
    public bool HasApiKey { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
