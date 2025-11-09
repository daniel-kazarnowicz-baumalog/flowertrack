namespace Flowertrack.Contracts.Users.Requests;

public record UpdateServiceUserRequest(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? Specialization);
