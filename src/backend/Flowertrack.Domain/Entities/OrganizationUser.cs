namespace Flowertrack.Domain.Entities;

/// <summary>
/// Placeholder entity for OrganizationUser profile.
/// Full implementation will be provided in Phase 1.1 (Issue #4).
/// </summary>
public class OrganizationUser
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid OrganizationId { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
