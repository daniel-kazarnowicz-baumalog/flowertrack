using Flowertrack.Domain.Common;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a client organization (customer) in the FLOWerTRACK system
/// </summary>
public sealed class Organization : AuditableEntity<Guid>
{
    // Private constructor for EF Core
    private Organization() : base(Guid.NewGuid()) { }
    
    /// <summary>
    /// Constructor for creating a new organization
    /// </summary>
    public Organization(string name, string email, string? phone = null) : base(Guid.NewGuid())
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        ServiceStatus = ServiceStatus.Active;
        ContractStartDate = DateTimeOffset.UtcNow;
    }
    
    /// <summary>
    /// Organization name (company name)
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Primary contact email
    /// </summary>
    public string Email { get; private set; } = string.Empty;
    
    /// <summary>
    /// Primary contact phone number
    /// </summary>
    public string? Phone { get; private set; }
    
    /// <summary>
    /// Address line
    /// </summary>
    public string? Address { get; private set; }
    
    /// <summary>
    /// City
    /// </summary>
    public string? City { get; private set; }
    
    /// <summary>
    /// Postal code
    /// </summary>
    public string? PostalCode { get; private set; }
    
    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; private set; }
    
    /// <summary>
    /// Service/contract status
    /// </summary>
    public ServiceStatus ServiceStatus { get; private set; }
    
    /// <summary>
    /// Contract start date
    /// </summary>
    public DateTimeOffset ContractStartDate { get; private set; }
    
    /// <summary>
    /// Contract end date (if applicable)
    /// </summary>
    public DateTimeOffset? ContractEndDate { get; private set; }
    
    /// <summary>
    /// API key for machine integration (generated token)
    /// </summary>
    public string? ApiKey { get; private set; }
    
    /// <summary>
    /// Additional notes about the organization
    /// </summary>
    public string? Notes { get; private set; }
    
    // Navigation properties
    // TODO: Add collections for Machines, Users, Tickets when those entities are created
    
    /// <summary>
    /// Update organization basic information
    /// </summary>
    public void UpdateBasicInfo(string name, string email, string? phone)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
    }
    
    /// <summary>
    /// Update organization address
    /// </summary>
    public void UpdateAddress(string? address, string? city, string? postalCode, string? country)
    {
        Address = address;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }
    
    /// <summary>
    /// Update service status
    /// </summary>
    public void UpdateServiceStatus(ServiceStatus status)
    {
        ServiceStatus = status;
    }
    
    /// <summary>
    /// Set contract dates
    /// </summary>
    public void SetContractPeriod(DateTimeOffset startDate, DateTimeOffset? endDate = null)
    {
        ContractStartDate = startDate;
        ContractEndDate = endDate;
    }
    
    /// <summary>
    /// Generate new API key for machine integration
    /// </summary>
    public void GenerateApiKey()
    {
        ApiKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()) + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
    
    /// <summary>
    /// Update notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        Notes = notes;
    }
}
