namespace Flowertrack.Contracts.Users.Responses;

/// <summary>
/// Organization user list item response
/// </summary>
public sealed record OrganizationUserListItemResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsActivated { get; init; }
    public int CreatedTicketsCount { get; init; }
    public DateTimeOffset? LastActivity { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
