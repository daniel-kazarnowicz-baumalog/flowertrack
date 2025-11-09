using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Entities.Users;

/// <summary>
/// Represents the many-to-many relationship between users and roles
/// </summary>
public sealed class UserRole : Entity<Guid>
{
    /// <summary>
    /// User identifier (can be ServiceUser or OrganizationUser)
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Role identifier
    /// </summary>
    public int RoleId { get; private set; }

    /// <summary>
    /// Date and time when the role was assigned
    /// </summary>
    public DateTimeOffset AssignedAt { get; private set; }

    /// <summary>
    /// User who assigned this role (optional)
    /// </summary>
    public Guid? AssignedBy { get; private set; }

    /// <summary>
    /// Navigation property to Role
    /// </summary>
    public Role Role { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private UserRole() : base(Guid.Empty)
    {
    }

    /// <summary>
    /// Creates a new user role assignment
    /// </summary>
    public static UserRole Create(Guid userId, int roleId, Guid? assignedBy = null)
    {
        var id = Guid.NewGuid();
        return new UserRole(id)
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = DateTimeOffset.UtcNow,
            AssignedBy = assignedBy
        };
    }

    /// <summary>
    /// Private constructor with ID for entity creation
    /// </summary>
    private UserRole(Guid id) : base(id)
    {
    }
}
