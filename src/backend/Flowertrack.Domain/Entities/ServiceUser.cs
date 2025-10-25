namespace Flowertrack.Domain.Entities;

/// <summary>
/// Placeholder entity for ServiceUser profile.
/// Full implementation will be provided in Phase 1.1 (Issue #4).
/// </summary>
public class ServiceUser
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Specialization { get; set; }
    public bool IsAvailable { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
