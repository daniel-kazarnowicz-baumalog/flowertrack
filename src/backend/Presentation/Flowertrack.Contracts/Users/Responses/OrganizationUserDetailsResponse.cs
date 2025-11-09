namespace Flowertrack.Contracts.Users.Responses;

public record OrganizationUserDetailsResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? PhoneNumber,
    string Role,
    string Status,
    bool IsActivated,
    int CreatedTicketsCount,
    DateTimeOffset? LastActivity,
    DateTimeOffset CreatedAt);
