namespace Flowertrack.Contracts.Users.Responses;

public record ServiceUserDetailsResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? PhoneNumber,
    string? Specialization,
    string Status,
    bool IsAvailable,
    int ActiveTicketsCount,
    int TotalTicketsCount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);
