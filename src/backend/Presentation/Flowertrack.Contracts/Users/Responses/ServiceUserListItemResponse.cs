namespace Flowertrack.Contracts.Users.Responses;

/// <summary>
/// Service user list item response
/// </summary>
public sealed record ServiceUserListItemResponse
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Specialization { get; init; }
    public string Status { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
    public int ActiveTicketsCount { get; init; }
    public DateTimeOffset? LastActivity { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
