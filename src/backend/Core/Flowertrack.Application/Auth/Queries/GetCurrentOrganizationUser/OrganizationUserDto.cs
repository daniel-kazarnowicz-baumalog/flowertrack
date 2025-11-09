namespace Flowertrack.Application.Auth.Queries.GetCurrentOrganizationUser;

/// <summary>
/// Organization user data transfer object for current user query
/// </summary>
public sealed record OrganizationUserDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string OrganizationName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsActivated { get; init; }
}

