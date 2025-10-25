namespace Flowertrack.Api.Domain.Enums;

/// <summary>
/// Valid roles for organization users
/// </summary>
public static class OrganizationUserRole
{
    /// <summary>
    /// Organization owner - full administrative access
    /// </summary>
    public const string Owner = "Owner";

    /// <summary>
    /// Organization administrator - can manage users and most settings
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Regular user - basic access to organization features
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// All valid roles
    /// </summary>
    public static readonly string[] ValidRoles = { Owner, Admin, User };

    /// <summary>
    /// Validates if a role is valid
    /// </summary>
    public static bool IsValid(string role)
    {
        return ValidRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
