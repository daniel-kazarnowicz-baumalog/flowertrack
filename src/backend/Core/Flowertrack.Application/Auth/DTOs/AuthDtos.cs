namespace Flowertrack.Application.Auth.DTOs;

/// <summary>
/// User information DTO for authentication responses
/// </summary>
public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string UserType, // "ServiceUser" or "OrganizationUser"
    IEnumerable<string> Roles,
    Guid? OrganizationId = null // Only for OrganizationUser
);

/// <summary>
/// Authentication response DTO
/// </summary>
public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    UserDto User
);
