namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Service for accessing current authenticated user information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// Gets the current user's organization ID (for organization users)
    /// </summary>
    Guid? OrganizationId { get; }

    /// <summary>
    /// Gets whether the current user is a service user (technician or admin)
    /// </summary>
    bool IsServiceUser { get; }

    /// <summary>
    /// Gets whether the current user is a service administrator
    /// </summary>
    bool IsServiceAdministrator { get; }

    /// <summary>
    /// Gets whether the current user is an organization administrator
    /// </summary>
    bool IsOrganizationAdministrator { get; }
}
