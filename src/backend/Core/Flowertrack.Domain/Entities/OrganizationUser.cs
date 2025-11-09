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
        IsActivated = false; // User must activate account
        SetCreatedAudit(userId);
    }

    /// <summary>
    /// User ID (internal domain ID)
    /// </summary>
    public new Guid Id { get; private set; }

    /// <summary>
    /// Supabase User ID (foreign key to auth.users)
    /// </summary>
    public Guid? SupabaseUserId { get; private set; }

    /// <summary>
    /// Indicates if the user has activated their account
    /// </summary>
    public bool IsActivated { get; private set; }

    /// <summary>
    /// Invitation token for account activation
    /// </summary>
    public string? InvitationToken { get; private set; }

    /// <summary>
    /// Invitation token expiration timestamp
    /// </summary>
    public DateTimeOffset? InvitationTokenExpiresAt { get; private set; }

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

    /// <summary>
    /// Password hash (for local authentication)
    /// </summary>
    public string? PasswordHash { get; private set; }

    // Navigation properties
    // public Organization? Organization { get; private set; }
    // public ICollection<Ticket> CreatedTickets { get; private set; } = new List<Ticket>();
    
    /// <summary>
    /// User roles
    /// </summary>
    private readonly List<Users.UserRole> _userRoles = new();
    public IReadOnlyCollection<Users.UserRole> UserRoles => _userRoles.AsReadOnly();

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
        if (Status == Enums.UserStatus.Active && IsActivated)
            return;

        Status = Enums.UserStatus.Active;
        IsActivated = true;
        InvitationToken = null; // Clear invitation token after activation
        InvitationTokenExpiresAt = null;
        SetUpdatedAudit(Id);

        RaiseDomainEvent(new OrganizationUserActivatedEvent(
            Id,
            OrganizationId,
            DateTimeOffset.UtcNow));
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
    /// Links this organization user to a Supabase auth user
    /// </summary>
    /// <param name="supabaseUserId">The Supabase auth user ID</param>
    public void LinkToSupabaseUser(Guid supabaseUserId)
    {
        if (supabaseUserId == Guid.Empty)
            throw new ArgumentException("Supabase User ID cannot be empty", nameof(supabaseUserId));

        if (SupabaseUserId.HasValue && SupabaseUserId.Value != supabaseUserId)
            throw new InvalidOperationException("Organization user is already linked to a different Supabase user");

        SupabaseUserId = supabaseUserId;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Sets invitation token for account activation
    /// </summary>
    /// <param name="token">Invitation token</param>
    /// <param name="expiresAt">Token expiration time</param>
    public void SetInvitationToken(string token, DateTimeOffset expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be empty", nameof(token));

        if (expiresAt <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Token expiration must be in the future", nameof(expiresAt));

        InvitationToken = token;
        InvitationTokenExpiresAt = expiresAt;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Validates if the invitation token is valid
    /// </summary>
    public bool IsInvitationTokenValid(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        if (InvitationToken != token)
            return false;

        if (!InvitationTokenExpiresAt.HasValue)
            return false;

        return InvitationTokenExpiresAt.Value > DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Get full name
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    /// <summary>
    /// Sets password hash
    /// </summary>
    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        PasswordHash = passwordHash;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Assigns a role to the user
    /// </summary>
    public void AssignRole(int roleId, Guid? assignedBy = null)
    {
        if (_userRoles.Any(ur => ur.RoleId == roleId))
            return; // Role already assigned

        var userRole = Users.UserRole.Create(Id, roleId, assignedBy);
        _userRoles.Add(userRole);
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Removes a role from the user
    /// </summary>
    public void RemoveRole(int roleId)
    {
        var userRole = _userRoles.FirstOrDefault(ur => ur.RoleId == roleId);
        if (userRole != null)
        {
            _userRoles.Remove(userRole);
            SetUpdatedAudit(Id);
        }
    }

    /// <summary>
    /// Gets all role names for this user
    /// </summary>
    public IEnumerable<string> GetRoleNames()
    {
        return _userRoles.Select(ur => ur.Role.Name);
    }

    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    public bool HasRole(int roleId)
    {
        return _userRoles.Any(ur => ur.RoleId == roleId);
    }

    /// <summary>
    /// Checks if user has any of the specified roles
    /// </summary>
    public bool HasAnyRole(params int[] roleIds)
    {
        return _userRoles.Any(ur => roleIds.Contains(ur.RoleId));
    }
}
