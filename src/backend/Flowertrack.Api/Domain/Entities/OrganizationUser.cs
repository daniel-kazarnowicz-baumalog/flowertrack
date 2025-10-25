using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Enums;
using Flowertrack.Api.Domain.Events;
using Flowertrack.Api.Exceptions;

namespace Flowertrack.Api.Domain.Entities;

/// <summary>
/// Represents a client organization user profile
/// Extends Supabase Auth users with business-specific data
/// </summary>
public class OrganizationUser : AuditableEntity<Guid>
{
    /// <summary>
    /// Reference to the user in Supabase Auth (auth.users)
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// User's first name
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// User's email address (synchronized with Supabase)
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Reference to the organization this user belongs to
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Current status of the user account
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// User's role within the organization (Owner, Admin, User)
    /// </summary>
    public string Role { get; private set; } = string.Empty;

    /// <summary>
    /// Reason for deactivation (if applicable)
    /// </summary>
    public string? DeactivationReason { get; private set; }

    // Private constructor for EF Core
    private OrganizationUser()
    {
    }

    /// <summary>
    /// Creates a new organization user
    /// </summary>
    public static OrganizationUser Create(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        Guid organizationId,
        string role = "User",
        string? phoneNumber = null,
        Guid? createdBy = null)
    {
        // Validate inputs
        if (userId == Guid.Empty)
            throw new ValidationException("UserId", "User ID cannot be empty");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ValidationException("FirstName", "First name is required");

        if (firstName.Length > 100)
            throw new ValidationException("FirstName", "First name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException("LastName", "Last name is required");

        if (lastName.Length > 100)
            throw new ValidationException("LastName", "Last name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Email", "Email is required");

        if (email.Length > 255)
            throw new ValidationException("Email", "Email cannot exceed 255 characters");

        if (!IsValidEmail(email))
            throw new ValidationException("Email", "Invalid email format");

        if (organizationId == Guid.Empty)
            throw new ValidationException("OrganizationId", "Organization ID is required");

        if (string.IsNullOrWhiteSpace(role))
            throw new ValidationException("Role", "Role is required");

        if (role.Length > 50)
            throw new ValidationException("Role", "Role cannot exceed 50 characters");

        if (!IsValidRole(role))
            throw new ValidationException("Role", "Invalid role. Must be Owner, Admin, or User");

        if (phoneNumber != null && phoneNumber.Length > 50)
            throw new ValidationException("PhoneNumber", "Phone number cannot exceed 50 characters");

        var organizationUser = new OrganizationUser
        {
            Id = userId,
            UserId = userId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            OrganizationId = organizationId,
            Role = role.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            Status = UserStatus.Pending
        };

        organizationUser.SetCreatedAudit(createdBy);
        organizationUser.AddDomainEvent(new OrganizationUserCreatedEvent(userId, email, firstName, lastName, organizationId));

        return organizationUser;
    }

    /// <summary>
    /// Activates the organization user account
    /// </summary>
    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new DomainException("User is already active");

        Status = UserStatus.Active;
        DeactivationReason = null;

        AddDomainEvent(new OrganizationUserActivatedEvent(UserId));
    }

    /// <summary>
    /// Deactivates the organization user account
    /// </summary>
    /// <param name="reason">Reason for deactivation</param>
    public void Deactivate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ValidationException("Reason", "Deactivation reason is required");

        if (Status == UserStatus.Deactivated)
            throw new DomainException("User is already deactivated");

        Status = UserStatus.Deactivated;
        DeactivationReason = reason.Trim();

        AddDomainEvent(new OrganizationUserDeactivatedEvent(UserId, reason));
    }

    /// <summary>
    /// Updates the user's profile information
    /// </summary>
    public void UpdateProfile(string firstName, string lastName, string? phoneNumber = null)
    {
        if (Status == UserStatus.Deactivated)
            throw new DomainException("Cannot update profile of a deactivated user");

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ValidationException("FirstName", "First name is required");

        if (firstName.Length > 100)
            throw new ValidationException("FirstName", "First name cannot exceed 100 characters");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ValidationException("LastName", "Last name is required");

        if (lastName.Length > 100)
            throw new ValidationException("LastName", "Last name cannot exceed 100 characters");

        if (phoneNumber != null && phoneNumber.Length > 50)
            throw new ValidationException("PhoneNumber", "Phone number cannot exceed 50 characters");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
    }

    /// <summary>
    /// Changes the organization this user belongs to
    /// </summary>
    public void ChangeOrganization(Guid newOrganizationId)
    {
        if (Status == UserStatus.Deactivated)
            throw new DomainException("Cannot change organization of a deactivated user");

        if (newOrganizationId == Guid.Empty)
            throw new ValidationException("OrganizationId", "Organization ID is required");

        if (OrganizationId == newOrganizationId)
            throw new DomainException("User is already in this organization");

        OrganizationId = newOrganizationId;
    }

    /// <summary>
    /// Updates the user's role within the organization
    /// </summary>
    public void UpdateRole(string role)
    {
        if (Status == UserStatus.Deactivated)
            throw new DomainException("Cannot update role of a deactivated user");

        if (string.IsNullOrWhiteSpace(role))
            throw new ValidationException("Role", "Role is required");

        if (role.Length > 50)
            throw new ValidationException("Role", "Role cannot exceed 50 characters");

        if (!IsValidRole(role))
            throw new ValidationException("Role", "Invalid role. Must be Owner, Admin, or User");

        var oldRole = Role;
        var newRole = role.Trim();

        if (oldRole != newRole)
        {
            Role = newRole;
            AddDomainEvent(new OrganizationUserRoleChangedEvent(UserId, oldRole, newRole));
        }
    }

    /// <summary>
    /// Gets the full name of the user
    /// </summary>
    public string GetFullName() => $"{FirstName} {LastName}";

    /// <summary>
    /// Validates email format
    /// </summary>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Validates role value
    /// </summary>
    private static bool IsValidRole(string role)
    {
        var validRoles = new[] { "Owner", "Admin", "User" };
        return validRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
}
