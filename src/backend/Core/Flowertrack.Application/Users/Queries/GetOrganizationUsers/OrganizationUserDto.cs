namespace Flowertrack.Application.Users.Queries.GetOrganizationUsers;

public class OrganizationUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActivated { get; set; }
    public int CreatedTicketsCount { get; set; }
    public DateTimeOffset? LastActivity { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
