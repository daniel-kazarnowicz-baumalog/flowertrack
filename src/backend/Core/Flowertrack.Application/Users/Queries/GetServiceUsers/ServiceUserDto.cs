namespace Flowertrack.Application.Users.Queries.GetServiceUsers;

public class ServiceUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? PhoneNumber { get; set; }
    public string? Specialization { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public int ActiveTicketsCount { get; set; }
    public DateTimeOffset? LastActivity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
