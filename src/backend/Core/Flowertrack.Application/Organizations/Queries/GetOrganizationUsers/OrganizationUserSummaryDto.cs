namespace Flowertrack.Application.Organizations.Queries.GetOrganizationUsers;

/// <summary>
/// DTO for organization user summary
/// </summary>
public sealed record OrganizationUserSummaryDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsActivated { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
