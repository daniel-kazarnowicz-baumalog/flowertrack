using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;
using Flowertrack.Domain.Events;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a service technician user profile
/// Extends Supabase Auth with business-specific data
/// </summary>
public sealed class ServiceUser : AuditableEntity<Guid>, IAggregateRoot
{
    // Private constructor for EF Core
    private ServiceUser() : base(Guid.Empty) { }

    // Private constructor for domain logic
    private ServiceUser(
        Guid userId,
        string firstName,
        string lastName,
        Email email,
        string? phoneNumber = null,
        string? specialization = null) : base(userId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required", nameof(lastName));

        Id = userId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Specialization = specialization;
        Status = Enums.UserStatus.Pending;
        IsAvailable = false;
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
    /// Phone number
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// User status
    /// </summary>
    public Enums.UserStatus Status { get; private set; }

    /// <summary>
    /// Specialization area (e.g., "Electrical", "Mechanical")
    /// </summary>
    public string? Specialization { get; private set; }

    /// <summary>
    /// Is available for ticket assignment
    /// </summary>
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// Password hash (for local authentication)
    /// </summary>
    public string? PasswordHash { get; private set; }

    // Navigation properties
    // public ICollection<Ticket> AssignedTickets { get; private set; } = new List<Ticket>();
    
    /// <summary>
    /// User roles
    /// </summary>
    private readonly List<Users.UserRole> _userRoles = new();
    public IReadOnlyCollection<Users.UserRole> UserRoles => _userRoles.AsReadOnly();

    /// <summary>
    /// Factory method to create a new service user
    /// </summary>
    public static ServiceUser Create(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? specialization = null)
    {
        var emailVO = Email.Create(email);
        var user = new ServiceUser(userId, firstName, lastName, emailVO, phoneNumber, specialization);

        user.RaiseDomainEvent(new ServiceUserCreatedEvent(
            userId,
            email,
            $"{firstName} {lastName}",
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
        IsAvailable = true;
        SetUpdatedAudit(Id);

        RaiseDomainEvent(new ServiceUserActivatedEvent(
            Id,
            DateTimeOffset.UtcNow,
            Id)); // Self-activation or by admin
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
        IsAvailable = false;
        SetUpdatedAudit(Id);

        RaiseDomainEvent(new ServiceUserDeactivatedEvent(
            Id,
            reason,
            DateTimeOffset.UtcNow,
            Id)); // Self-deactivation or by admin
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
    /// Set availability for ticket assignment
    /// </summary>
    public void SetAvailability(bool isAvailable)
    {
        if (Status != Enums.UserStatus.Active)
            throw new InvalidOperationException("Cannot change availability of inactive user");

        IsAvailable = isAvailable;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Update specialization
    /// </summary>
    public void UpdateSpecialization(string? specialization)
    {
        Specialization = specialization;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Set the Supabase User ID for authentication linking
    /// </summary>
    public void SetSupabaseUserId(Guid supabaseUserId)
    {
        if (SupabaseUserId.HasValue)
            throw new InvalidOperationException("Supabase User ID is already set");
        
        SupabaseUserId = supabaseUserId;
        SetUpdatedAudit(Id);
    }

    /// <summary>
    /// Links this service user to a Supabase auth user
    /// </summary>
    /// <param name="supabaseUserId">The Supabase auth user ID</param>
    public void LinkToSupabaseUser(Guid supabaseUserId)
    {
        if (supabaseUserId == Guid.Empty)
            throw new ArgumentException("Supabase User ID cannot be empty", nameof(supabaseUserId));

        if (SupabaseUserId.HasValue && SupabaseUserId.Value != supabaseUserId)
            throw new InvalidOperationException("Service user is already linked to a different Supabase user");

        SupabaseUserId = supabaseUserId;
        SetUpdatedAudit(Id);
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
