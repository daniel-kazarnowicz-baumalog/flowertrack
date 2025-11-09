using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Entities.Users;

/// <summary>
/// Represents a user role in the system (dictionary/lookup table)
/// </summary>
public sealed class Role : Entity<int>
{
    /// <summary>
    /// Role name
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Role description
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Role() : base(0)
    {
    }

    /// <summary>
    /// Private constructor for predefined roles
    /// </summary>
    private Role(int id, string name, string description) : base(id)
    {
        Name = name;
        Description = description;
    }

    // Predefined roles
    public static readonly Role ServiceAdministrator = new(1, "ServiceAdministrator", "Service team administrator with full access to service management");
    public static readonly Role ServiceTechnician = new(2, "ServiceTechnician", "Service technician with ticket management and resolution capabilities");
    public static readonly Role OrganizationAdministrator = new(3, "OrganizationAdministrator", "Organization administrator with team and machine management access");
    public static readonly Role Operator = new(4, "Operator", "Machine operator with basic ticket creation and viewing capabilities");

    /// <summary>
    /// Gets all predefined roles
    /// </summary>
    public static IEnumerable<Role> GetAllRoles()
    {
        yield return ServiceAdministrator;
        yield return ServiceTechnician;
        yield return OrganizationAdministrator;
        yield return Operator;
    }
}
