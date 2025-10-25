using Flowertrack.Domain.Common;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Events;
using System.Security.Cryptography;

namespace Flowertrack.Domain.Entities.Organizations;

/// <summary>
/// Represents an organization in the system.
/// Organizations are companies that use the service ticket management system.
/// </summary>
public sealed class Organization : AuditableEntity<Guid>, IAggregateRoot
{
    // Private constructor for EF Core
    private Organization()
    {
        Name = string.Empty;
    }

    /// <summary>
    /// Gets the name of the organization.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the email address of the organization.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Gets the phone number of the organization.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the street address of the organization.
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Gets the city where the organization is located.
    /// </summary>
    public string? City { get; private set; }

    /// <summary>
    /// Gets the postal code of the organization.
    /// </summary>
    public string? PostalCode { get; private set; }

    /// <summary>
    /// Gets the country where the organization is located.
    /// </summary>
    public string? Country { get; private set; }

    /// <summary>
    /// Gets the current service status of the organization.
    /// </summary>
    public ServiceStatus ServiceStatus { get; private set; }

    /// <summary>
    /// Gets the start date of the organization's contract.
    /// </summary>
    public DateTimeOffset? ContractStartDate { get; private set; }

    /// <summary>
    /// Gets the end date of the organization's contract.
    /// </summary>
    public DateTimeOffset? ContractEndDate { get; private set; }

    /// <summary>
    /// Gets the API key for the organization (used for machine integration).
    /// </summary>
    public string? ApiKey { get; private set; }

    /// <summary>
    /// Gets optional notes about the organization.
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Creates a new organization.
    /// </summary>
    /// <param name="name">The name of the organization.</param>
    /// <param name="email">The email address of the organization.</param>
    /// <param name="phone">The phone number of the organization.</param>
    /// <param name="address">The street address of the organization.</param>
    /// <param name="city">The city where the organization is located.</param>
    /// <param name="postalCode">The postal code of the organization.</param>
    /// <param name="country">The country where the organization is located.</param>
    /// <param name="notes">Optional notes about the organization.</param>
    /// <returns>A new organization instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the name is null or whitespace.</exception>
    public static Organization Create(
        string name,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? city = null,
        string? postalCode = null,
        string? country = null,
        string? notes = null)
    {
        // Guard clauses
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Organization name cannot be empty.", nameof(name));
        }

        if (name.Length > 255)
        {
            throw new ArgumentException("Organization name cannot exceed 255 characters.", nameof(name));
        }

        if (email?.Length > 255)
        {
            throw new ArgumentException("Email cannot exceed 255 characters.", nameof(email));
        }

        if (phone?.Length > 50)
        {
            throw new ArgumentException("Phone cannot exceed 50 characters.", nameof(phone));
        }

        if (address?.Length > 255)
        {
            throw new ArgumentException("Address cannot exceed 255 characters.", nameof(address));
        }

        if (city?.Length > 100)
        {
            throw new ArgumentException("City cannot exceed 100 characters.", nameof(city));
        }

        if (postalCode?.Length > 20)
        {
            throw new ArgumentException("Postal code cannot exceed 20 characters.", nameof(postalCode));
        }

        if (country?.Length > 100)
        {
            throw new ArgumentException("Country cannot exceed 100 characters.", nameof(country));
        }

        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Email = email?.Trim(),
            Phone = phone?.Trim(),
            Address = address?.Trim(),
            City = city?.Trim(),
            PostalCode = postalCode?.Trim(),
            Country = country?.Trim(),
            ServiceStatus = ServiceStatus.Active,
            Notes = notes?.Trim()
        };

        organization.AddDomainEvent(new OrganizationCreatedEvent(organization.Id, organization.Name));

        return organization;
    }

    /// <summary>
    /// Updates the service status of the organization.
    /// </summary>
    /// <param name="status">The new service status.</param>
    /// <param name="reason">The reason for the status change.</param>
    /// <exception cref="ArgumentException">Thrown when the reason is null or whitespace.</exception>
    public void UpdateServiceStatus(ServiceStatus status, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for status change cannot be empty.", nameof(reason));
        }

        if (ServiceStatus == status)
        {
            return; // No change needed
        }

        var previousStatus = ServiceStatus;
        ServiceStatus = status;

        AddDomainEvent(new OrganizationServiceStatusChangedEvent(
            Id,
            previousStatus,
            status,
            reason));
    }

    /// <summary>
    /// Suspends the organization's service.
    /// </summary>
    /// <param name="reason">The reason for the suspension.</param>
    /// <exception cref="ArgumentException">Thrown when the reason is null or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the service is already suspended.</exception>
    public void SuspendService(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for suspension cannot be empty.", nameof(reason));
        }

        if (ServiceStatus == ServiceStatus.Suspended)
        {
            throw new InvalidOperationException("Service is already suspended.");
        }

        var previousStatus = ServiceStatus;
        ServiceStatus = ServiceStatus.Suspended;

        AddDomainEvent(new OrganizationServiceSuspendedEvent(Id, reason));
        AddDomainEvent(new OrganizationServiceStatusChangedEvent(
            Id,
            previousStatus,
            ServiceStatus.Suspended,
            reason));
    }

    /// <summary>
    /// Reactivates the organization's service.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the service is not suspended or has an expired contract.</exception>
    public void ReactivateService()
    {
        if (ServiceStatus != ServiceStatus.Suspended)
        {
            throw new InvalidOperationException("Only suspended services can be reactivated.");
        }

        // Check if contract is still valid
        if (ContractEndDate.HasValue && ContractEndDate.Value < DateTimeOffset.UtcNow)
        {
            throw new InvalidOperationException("Cannot reactivate service with an expired contract. Renew the contract first.");
        }

        var previousStatus = ServiceStatus;
        ServiceStatus = ServiceStatus.Active;

        AddDomainEvent(new OrganizationServiceStatusChangedEvent(
            Id,
            previousStatus,
            ServiceStatus.Active,
            "Service reactivated"));
    }

    /// <summary>
    /// Updates the contact information of the organization.
    /// </summary>
    /// <param name="email">The new email address.</param>
    /// <param name="phone">The new phone number.</param>
    /// <param name="address">The new street address.</param>
    /// <exception cref="ArgumentException">Thrown when any parameter exceeds maximum length.</exception>
    public void UpdateContactInfo(string? email, string? phone, string? address)
    {
        if (email?.Length > 255)
        {
            throw new ArgumentException("Email cannot exceed 255 characters.", nameof(email));
        }

        if (phone?.Length > 50)
        {
            throw new ArgumentException("Phone cannot exceed 50 characters.", nameof(phone));
        }

        if (address?.Length > 255)
        {
            throw new ArgumentException("Address cannot exceed 255 characters.", nameof(address));
        }

        Email = email?.Trim();
        Phone = phone?.Trim();
        Address = address?.Trim();
    }

    /// <summary>
    /// Renews the organization's contract.
    /// </summary>
    /// <param name="newEndDate">The new contract end date.</param>
    /// <exception cref="ArgumentException">Thrown when the new end date is in the past or not after the current end date.</exception>
    public void RenewContract(DateTimeOffset newEndDate)
    {
        if (newEndDate <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentException("Contract end date must be in the future.", nameof(newEndDate));
        }

        if (ContractEndDate.HasValue && newEndDate <= ContractEndDate.Value)
        {
            throw new ArgumentException("New contract end date must be after the current end date.", nameof(newEndDate));
        }

        var previousEndDate = ContractEndDate;
        
        // If this is the first contract, set the start date
        if (!ContractStartDate.HasValue)
        {
            ContractStartDate = DateTimeOffset.UtcNow;
        }

        ContractEndDate = newEndDate;

        // If the organization was expired, reactivate it
        if (ServiceStatus == ServiceStatus.Expired)
        {
            ServiceStatus = ServiceStatus.Active;
            AddDomainEvent(new OrganizationServiceStatusChangedEvent(
                Id,
                ServiceStatus.Expired,
                ServiceStatus.Active,
                "Contract renewed"));
        }

        AddDomainEvent(new OrganizationContractRenewedEvent(Id, previousEndDate, newEndDate));
    }

    /// <summary>
    /// Generates a new API key for the organization.
    /// </summary>
    /// <returns>The generated API key.</returns>
    public string GenerateApiKey()
    {
        var isRegeneration = !string.IsNullOrEmpty(ApiKey);

        // Generate a secure random API key
        var randomBytes = new byte[32];
        RandomNumberGenerator.Fill(randomBytes);

        ApiKey = Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        AddDomainEvent(new OrganizationApiKeyGeneratedEvent(Id, isRegeneration));

        return ApiKey;
    }

    /// <summary>
    /// Checks if the organization's contract has expired and updates status if needed.
    /// This method should be called periodically or before operations that depend on contract status.
    /// </summary>
    public void CheckContractExpiration()
    {
        if (ContractEndDate.HasValue && 
            ContractEndDate.Value < DateTimeOffset.UtcNow && 
            ServiceStatus != ServiceStatus.Expired)
        {
            UpdateServiceStatus(ServiceStatus.Expired, "Contract expired");
        }
    }

    /// <summary>
    /// Determines whether the organization can register new machines.
    /// </summary>
    /// <returns>True if machines can be registered; otherwise, false.</returns>
    public bool CanRegisterMachines()
    {
        return ServiceStatus == ServiceStatus.Active;
    }
}
