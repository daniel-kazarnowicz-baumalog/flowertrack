using Flowertrack.Domain.Common;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.Exceptions;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a service technician user profile
/// Extends Supabase Auth users with business-specific data
/// </summary>
public class ServiceUser : AuditableEntity<Guid>
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
    /// User's phone number
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Current status of the user account
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Area of specialization for the service technician
    /// </summary>
    public string? Specialization { get; private set; }

    /// <summary>
    /// Indicates whether the technician is available for ticket assignment
    /// </summary>
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// Reason for deactivation (if applicable)
    /// </summary>
    public string? DeactivationReason { get; private set; }

    // Private constructor for EF Core
    private ServiceUser()
    {
    }

    /// <summary>
    /// Creates a new service user
    /// </summary>
    public static ServiceUser Create(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? specialization = null,
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

        if (phoneNumber != null && phoneNumber.Length > 50)
            throw new ValidationException("PhoneNumber", "Phone number cannot exceed 50 characters");

        if (specialization != null && specialization.Length > 255)
            throw new ValidationException("Specialization", "Specialization cannot exceed 255 characters");

        var serviceUser = new ServiceUser
        {
            Id = userId,
            UserId = userId,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Email = email.Trim().ToLowerInvariant(),
            PhoneNumber = phoneNumber?.Trim(),
            Specialization = specialization?.Trim(),
            Status = UserStatus.Pending,
            IsAvailable = false
        };

        serviceUser.SetCreatedAudit(createdBy);
        serviceUser.AddDomainEvent(new ServiceUserCreatedEvent(userId, email, firstName, lastName));

        return serviceUser;
    }

    /// <summary>
    /// Activates the service user account
    /// </summary>
    public void Activate()
    {
        if (Status == UserStatus.Active)
            throw new DomainException("User is already active");

        Status = UserStatus.Active;
        IsAvailable = true;
        DeactivationReason = null;

        AddDomainEvent(new ServiceUserActivatedEvent(UserId));
    }

    /// <summary>
    /// Deactivates the service user account
    /// </summary>
    /// <param name="reason">Reason for deactivation</param>
    public void Deactivate(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ValidationException("Reason", "Deactivation reason is required");

        if (Status == UserStatus.Deactivated)
            throw new DomainException("User is already deactivated");

        Status = UserStatus.Deactivated;
        IsAvailable = false;
        DeactivationReason = reason.Trim();

        AddDomainEvent(new ServiceUserDeactivatedEvent(UserId, reason));
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
    /// Sets the availability status of the service user
    /// </summary>
    public void SetAvailability(bool isAvailable)
    {
        if (Status == UserStatus.Deactivated)
            throw new DomainException("Cannot change availability of a deactivated user");

        if (IsAvailable != isAvailable)
        {
            IsAvailable = isAvailable;
            AddDomainEvent(new ServiceUserAvailabilityChangedEvent(UserId, isAvailable));
        }
    }

    /// <summary>
    /// Updates the specialization of the service user
    /// </summary>
    public void UpdateSpecialization(string specialization)
    {
        if (Status == UserStatus.Deactivated)
            throw new DomainException("Cannot update specialization of a deactivated user");

        if (string.IsNullOrWhiteSpace(specialization))
            throw new ValidationException("Specialization", "Specialization cannot be empty");

        if (specialization.Length > 255)
            throw new ValidationException("Specialization", "Specialization cannot exceed 255 characters");

        Specialization = specialization.Trim();
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
}
