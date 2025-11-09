namespace Flowertrack.Application.Auth.Queries.GetCurrentServiceUser;

/// <summary>
/// Service user data transfer object for current user query
/// </summary>
public sealed record ServiceUserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

