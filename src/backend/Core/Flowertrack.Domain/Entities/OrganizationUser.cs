using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a client organization user profile
/// Extends Supabase Auth with business-specific data
/// </summary>
public sealed class OrganizationUser : AuditableEntity<Guid>, IAggregateRoot
{
    // Private constructor for EF Core
    private OrganizationUser() : base(Guid.Empty) { }

    // Private constructor for domain logic
    private OrganizationUser(
        Guid userId,
        string firstName,
        string lastName,
        Email email,
        Guid organizationId,
        string role,
        string? phoneNumber = null) : base(userId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required", nameof(organizationId));

        Id = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        OrganizationId = organizationId;
        Role = role;
        PhoneNumber = phoneNumber;
        Status = Enums.UserStatus.Pending;
        SetCreatedAudit(userId);
    }

    /// <summary>
    /// User ID (matches Supabase auth.users.id)
    /// </summary>
    public new Guid Id { get; private set; }

    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Email address (synced with Supabase auth)
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Organization ID this user belongs to
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// User status
    /// </summary>
    public Enums.UserStatus Status { get; private set; }

    /// <summary>
    /// Role in the organization (Owner, Admin, User)
    /// </summary>
    public string Role { get; private set; } = string.Empty;

    // Navigation properties
    // public Organization? Organization { get; private set; }
    // public ICollection<Ticket> CreatedTickets { get; private set; } = new List<Ticket>();

    /// <summary>
    /// Factory method to create a new organization user
    /// </summary>
    public static OrganizationUser Create(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        Guid organizationId,
        string role = Enums.OrganizationUserRole.User,
        string? phoneNumber = null)
    {
        if (!Enums.OrganizationUserRole.IsValid(role))
            throw new ArgumentException($"Invalid role: {role}", nameof(role));

        var emailVO = Email.Create(email);
        var user = new OrganizationUser(userId, firstName, lastName, emailVO, organizationId, role, phoneNumber);

        user.RaiseDomainEvent(new OrganizationUserCreatedEvent(
            userId,
            organizationId,
            email,
            role,
            DateTimeOffset.UtcNow));

        return user;
    }

    /// <summary>
    /// Activate the user account
    /// </summary>
    public void Activate()
    {
        if (Status == Enums.UserStatus.Active)
            return;

        Status = Enums.UserStatus.Active;
        SetUpdatedAudit(Id);

        // Event for activation can be added if needed
    }

    /// <summary>
    /// Deactivate the user account
    /// </summary>
    public void Deactivate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required for deactivation", nameof(reason));

        if (Status == Enums.UserStatus.Inactive)
            return;

        Status = Enums.UserStatus.Inactive;
        SetUpdatedAudit(Id);

        // Event for deactivation can be added if needed
    }

    /// <summary>
    /// Update user profile information
    /// </summary>
    public void UpdateProfile(string firstName, string lastName, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Change user's organization
    /// </summary>
    public void ChangeOrganization(Guid newOrganizationId)
    {
        if (newOrganizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required", nameof(newOrganizationId));

        if (OrganizationId == newOrganizationId)
            return;

        OrganizationId = newOrganizationId;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Update user's role in the organization
    /// </summary>
    public void UpdateRole(string newRole)
    {
        if (!Enums.OrganizationUserRole.IsValid(newRole))
            throw new ArgumentException($"Invalid role: {newRole}", nameof(newRole));

        if (Role == newRole)
            return;

        var oldRole = Role;
        Role = newRole;
        SetUpdatedAudit(Id);

        RaiseDomainEvent(new OrganizationUserRoleChangedEvent(
            Id,
            oldRole,
            newRole,
            DateTimeOffset.UtcNow,
            Id));
    }

    /// <summary>
    /// Get full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";
}
