using System.Security.Cryptography;
using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Enums;
using Flowertrack.Api.Domain.Events;
using Flowertrack.Api.Domain.ValueObjects;

namespace Flowertrack.Api.Domain.Entities;

/// <summary>
/// Represents an organization in the system
/// </summary>
public sealed class Organization : AuditableEntity<Guid>, IAggregateRoot
{
    /// <summary>
    /// Gets the organization name
    /// </summary>
    public string Name { get; private set; } = default!;

    /// <summary>
    /// Gets the organization email
    /// </summary>
    public Email Email { get; private set; } = default!;

    /// <summary>
    /// Gets the organization phone number
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the organization address
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Gets the organization city
    /// </summary>
    public string? City { get; private set; }

    /// <summary>
    /// Gets the organization postal code
    /// </summary>
    public string? PostalCode { get; private set; }

    /// <summary>
    /// Gets the organization country
    /// </summary>
    public string? Country { get; private set; }

    /// <summary>
    /// Gets the service status of the organization
    /// </summary>
    public ServiceStatus ServiceStatus { get; private set; }

    /// <summary>
    /// Gets the contract start date
    /// </summary>
    public DateTimeOffset? ContractStartDate { get; private set; }

    /// <summary>
    /// Gets the contract end date
    /// </summary>
    public DateTimeOffset? ContractEndDate { get; private set; }

    /// <summary>
    /// Gets the API key for integration
    /// </summary>
    public string? ApiKey { get; private set; }

    /// <summary>
    /// Gets optional notes about the organization
    /// </summary>
    public string? Notes { get; private set; }

    // Navigation properties
    // Note: These would be properly configured with EF Core relationships
    // For now, we define them as they would be used in the domain model
    // private readonly List<Machine> _machines = new();
    // public IReadOnlyCollection<Machine> Machines => _machines.AsReadOnly();

    // private readonly List<Ticket> _tickets = new();
    // public IReadOnlyCollection<Ticket> Tickets => _tickets.AsReadOnly();

    // private readonly List<User> _users = new();
    // public IReadOnlyCollection<User> Users => _users.AsReadOnly();

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Organization()
    {
    }

    /// <summary>
    /// Private constructor for creating organization
    /// </summary>
    private Organization(
        string name,
        Email email,
        string? phone,
        string? address,
        string? city,
        string? postalCode,
        string? country,
        ServiceStatus serviceStatus,
        DateTimeOffset? contractStartDate,
        DateTimeOffset? contractEndDate,
        string? notes,
        string? createdBy)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Phone = phone;
        Address = address;
        City = city;
        PostalCode = postalCode;
        Country = country;
        ServiceStatus = serviceStatus;
        ContractStartDate = contractStartDate;
        ContractEndDate = contractEndDate;
        Notes = notes;

        SetCreatedAudit(createdBy);

        // Validate contract dates if both are provided
        ValidateContractDates();

        AddDomainEvent(new OrganizationCreatedEvent(Id, Name, Email.Value));
    }

    /// <summary>
    /// Creates a new organization
    /// </summary>
    /// <param name="name">Organization name</param>
    /// <param name="email">Organization email</param>
    /// <param name="phone">Organization phone</param>
    /// <param name="address">Organization address</param>
    /// <param name="city">Organization city</param>
    /// <param name="postalCode">Organization postal code</param>
    /// <param name="country">Organization country</param>
    /// <param name="serviceStatus">Initial service status</param>
    /// <param name="contractStartDate">Contract start date</param>
    /// <param name="contractEndDate">Contract end date</param>
    /// <param name="notes">Optional notes</param>
    /// <param name="createdBy">User creating the organization</param>
    /// <returns>A new Organization instance</returns>
    public static Organization Create(
        string name,
        string email,
        string? phone = null,
        string? address = null,
        string? city = null,
        string? postalCode = null,
        string? country = null,
        ServiceStatus serviceStatus = ServiceStatus.Active,
        DateTimeOffset? contractStartDate = null,
        DateTimeOffset? contractEndDate = null,
        string? notes = null,
        string? createdBy = null)
    {
        // Guard clauses
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Organization name cannot be empty", nameof(name));
        }

        if (name.Length > 255)
        {
            throw new ArgumentException("Organization name cannot exceed 255 characters", nameof(name));
        }

        var emailValueObject = Email.Create(email);

        // Validate optional fields
        ValidateOptionalField(phone, 50, nameof(phone));
        ValidateOptionalField(address, 255, nameof(address));
        ValidateOptionalField(city, 100, nameof(city));
        ValidateOptionalField(postalCode, 20, nameof(postalCode));
        ValidateOptionalField(country, 100, nameof(country));

        return new Organization(
            name,
            emailValueObject,
            phone,
            address,
            city,
            postalCode,
            country,
            serviceStatus,
            contractStartDate,
            contractEndDate,
            notes,
            createdBy);
    }

    /// <summary>
    /// Updates the service status of the organization
    /// </summary>
    /// <param name="status">New service status</param>
    /// <param name="reason">Reason for the status change</param>
    /// <param name="updatedBy">User making the change</param>
    public void UpdateServiceStatus(ServiceStatus status, string reason, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for status change cannot be empty", nameof(reason));
        }

        if (ServiceStatus == status)
        {
            return; // No change needed
        }

        var previousStatus = ServiceStatus;
        ServiceStatus = status;

        SetUpdatedAudit(updatedBy);

        AddDomainEvent(new OrganizationServiceStatusChangedEvent(Id, previousStatus, status, reason));
    }

    /// <summary>
    /// Suspends the organization's service
    /// </summary>
    /// <param name="reason">Reason for suspension</param>
    /// <param name="updatedBy">User making the change</param>
    public void SuspendService(string reason, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for suspension cannot be empty", nameof(reason));
        }

        if (ServiceStatus == ServiceStatus.Suspended)
        {
            return; // Already suspended
        }

        var previousStatus = ServiceStatus;
        ServiceStatus = ServiceStatus.Suspended;

        SetUpdatedAudit(updatedBy);

        AddDomainEvent(new OrganizationServiceSuspendedEvent(Id, reason));
        AddDomainEvent(new OrganizationServiceStatusChangedEvent(Id, previousStatus, ServiceStatus.Suspended, reason));
    }

    /// <summary>
    /// Reactivates the organization's service
    /// </summary>
    /// <param name="updatedBy">User making the change</param>
    public void ReactivateService(string? updatedBy = null)
    {
        if (ServiceStatus == ServiceStatus.Active)
        {
            return; // Already active
        }

        // Business rule: Cannot reactivate if contract is expired
        if (ContractEndDate.HasValue && ContractEndDate.Value < DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("Cannot reactivate organization with expired contract. Please renew the contract first.");
        }

        var previousStatus = ServiceStatus;
        ServiceStatus = ServiceStatus.Active;

        SetUpdatedAudit(updatedBy);

        AddDomainEvent(new OrganizationServiceStatusChangedEvent(Id, previousStatus, ServiceStatus.Active, "Service reactivated"));
    }

    /// <summary>
    /// Updates the organization's contact information
    /// </summary>
    /// <param name="email">New email address</param>
    /// <param name="phone">New phone number</param>
    /// <param name="address">New address</param>
    /// <param name="updatedBy">User making the change</param>
    public void UpdateContactInfo(string email, string? phone, string? address, string? updatedBy = null)
    {
        Email = Email.Create(email);
        Phone = phone;
        Address = address;

        ValidateOptionalField(phone, 50, nameof(phone));
        ValidateOptionalField(address, 255, nameof(address));

        SetUpdatedAudit(updatedBy);
    }

    /// <summary>
    /// Renews the organization's contract
    /// </summary>
    /// <param name="newEndDate">New contract end date</param>
    /// <param name="updatedBy">User making the change</param>
    public void RenewContract(DateTimeOffset newEndDate, string? updatedBy = null)
    {
        if (newEndDate <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("Contract end date must be in the future", nameof(newEndDate));
        }

        if (ContractStartDate.HasValue && newEndDate <= ContractStartDate.Value)
        {
            throw new ArgumentException("Contract end date must be after start date", nameof(newEndDate));
        }

        var previousEndDate = ContractEndDate;
        ContractEndDate = newEndDate;

        // If contract was expired, reactivate the service
        if (ServiceStatus == ServiceStatus.Expired)
        {
            ServiceStatus = ServiceStatus.Active;
            AddDomainEvent(new OrganizationServiceStatusChangedEvent(Id, ServiceStatus.Expired, ServiceStatus.Active, "Contract renewed"));
        }

        SetUpdatedAudit(updatedBy);

        AddDomainEvent(new OrganizationContractRenewedEvent(Id, previousEndDate, newEndDate));
    }

    /// <summary>
    /// Generates a new API key for the organization
    /// </summary>
    /// <param name="updatedBy">User making the change</param>
    /// <returns>The generated API key</returns>
    public string GenerateApiKey(string? updatedBy = null)
    {
        var isRegeneration = !string.IsNullOrEmpty(ApiKey);

        // Generate a secure random API key
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        ApiKey = $"ft_{Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=')}";

        SetUpdatedAudit(updatedBy);

        AddDomainEvent(new OrganizationApiKeyGeneratedEvent(Id, isRegeneration));

        return ApiKey;
    }

    /// <summary>
    /// Checks if the organization can register new machines
    /// Business rule: Suspended organizations cannot register new machines
    /// </summary>
    /// <returns>True if machines can be registered; otherwise, false</returns>
    public bool CanRegisterMachines()
    {
        return ServiceStatus != ServiceStatus.Suspended && ServiceStatus != ServiceStatus.Inactive;
    }

    /// <summary>
    /// Checks if the contract has expired and updates status if needed
    /// Business rule: Expired contracts automatically change status to Expired
    /// </summary>
    /// <param name="updatedBy">User making the check</param>
    public void CheckAndUpdateContractStatus(string? updatedBy = null)
    {
        if (!ContractEndDate.HasValue)
        {
            return;
        }

        if (ContractEndDate.Value < DateTimeOffset.UtcNow && ServiceStatus != ServiceStatus.Expired)
        {
            var previousStatus = ServiceStatus;
            ServiceStatus = ServiceStatus.Expired;

            SetUpdatedAudit(updatedBy);

            AddDomainEvent(new OrganizationServiceStatusChangedEvent(Id, previousStatus, ServiceStatus.Expired, "Contract has expired"));
        }
    }

    /// <summary>
    /// Validates contract dates
    /// </summary>
    private void ValidateContractDates()
    {
        if (ContractStartDate.HasValue && ContractEndDate.HasValue)
        {
            if (ContractEndDate.Value <= ContractStartDate.Value)
            {
                throw new ArgumentException("Contract end date must be after start date");
            }
        }
    }

    /// <summary>
    /// Validates an optional string field
    /// </summary>
    private static void ValidateOptionalField(string? value, int maxLength, string fieldName)
    {
        if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} cannot exceed {maxLength} characters", fieldName);
        }
    }
}
