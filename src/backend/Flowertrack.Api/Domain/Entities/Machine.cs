using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.ValueObjects;
using Flowertrack.Api.Domain.Events;

namespace Flowertrack.Api.Domain.Entities;

/// <summary>
/// Represents a production machine/equipment registered in the system
/// </summary>
public sealed class Machine : AuditableEntity<Guid>, IAggregateRoot
{
    /// <summary>
    /// The organization that owns this machine
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// Unique serial number of the machine
    /// </summary>
    public string SerialNumber { get; private set; } = string.Empty;

    /// <summary>
    /// Brand/manufacturer of the machine
    /// </summary>
    public string? Brand { get; private set; }

    /// <summary>
    /// Model name/number of the machine
    /// </summary>
    public string? Model { get; private set; }

    /// <summary>
    /// Physical location of the machine
    /// </summary>
    public string? Location { get; private set; }

    /// <summary>
    /// Current operational status of the machine
    /// </summary>
    public MachineStatus Status { get; private set; }

    /// <summary>
    /// API token for machine authentication and telemetry
    /// </summary>
    public MachineApiKey? ApiToken { get; private set; }

    /// <summary>
    /// Date of last completed maintenance
    /// </summary>
    public DateOnly? LastMaintenanceDate { get; private set; }

    /// <summary>
    /// Date when next maintenance is scheduled
    /// </summary>
    public DateOnly? NextMaintenanceDate { get; private set; }

    /// <summary>
    /// ID of the maintenance interval configuration
    /// </summary>
    public int? MaintenanceIntervalId { get; private set; }

    // Navigation properties (for EF Core)
    // public Organization? Organization { get; private set; }
    // public ICollection<Ticket> Tickets { get; private set; } = new List<Ticket>();

    // Private constructor for EF Core
    private Machine()
    {
    }

    // Private constructor for domain logic
    private Machine(
        Guid organizationId,
        string serialNumber,
        string? brand,
        string? model,
        string? location,
        string? createdBy)
    {
        if (organizationId == Guid.Empty)
            throw new ArgumentException("Organization ID is required", nameof(organizationId));

        if (string.IsNullOrWhiteSpace(serialNumber))
            throw new ArgumentException("Serial number is required", nameof(serialNumber));

        if (serialNumber.Length > 255)
            throw new ArgumentException("Serial number cannot exceed 255 characters", nameof(serialNumber));

        if (brand?.Length > 100)
            throw new ArgumentException("Brand cannot exceed 100 characters", nameof(brand));

        if (model?.Length > 100)
            throw new ArgumentException("Model cannot exceed 100 characters", nameof(model));

        if (location?.Length > 255)
            throw new ArgumentException("Location cannot exceed 255 characters", nameof(location));

        Id = Guid.NewGuid();
        OrganizationId = organizationId;
        SerialNumber = serialNumber;
        Brand = brand;
        Model = model;
        Location = location;
        Status = MachineStatus.Inactive;
        SetCreatedAudit(createdBy);

        RaiseDomainEvent(new MachineRegisteredEvent(
            Id,
            OrganizationId,
            SerialNumber,
            Brand,
            Model));
    }

    /// <summary>
    /// Create a new machine
    /// </summary>
    public static Machine Create(
        Guid organizationId,
        string serialNumber,
        string? brand = null,
        string? model = null,
        string? location = null,
        string? createdBy = null)
    {
        return new Machine(organizationId, serialNumber, brand, model, location, createdBy);
    }

    /// <summary>
    /// Generate a new API token for the machine
    /// </summary>
    public void GenerateApiToken(string? updatedBy = null)
    {
        ApiToken = MachineApiKey.Generate();
        SetUpdatedAudit(updatedBy);

        RaiseDomainEvent(new MachineApiTokenGeneratedEvent(
            Id,
            isRegeneration: false,
            reason: "Initial API token generation"));
    }

    /// <summary>
    /// Regenerate the API token (e.g., after security compromise)
    /// </summary>
    public void RegenerateApiToken(string reason, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required for token regeneration", nameof(reason));

        ApiToken = MachineApiKey.Generate();
        SetUpdatedAudit(updatedBy);

        RaiseDomainEvent(new MachineApiTokenGeneratedEvent(
            Id,
            isRegeneration: true,
            reason: reason));
    }

    /// <summary>
    /// Update the machine's operational status
    /// </summary>
    public void UpdateStatus(MachineStatus newStatus, string reason, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required for status change", nameof(reason));

        // Business rule: Machine in Alarm status cannot be set to Active without clearing the alarm first
        if (Status == MachineStatus.Alarm && newStatus == MachineStatus.Active)
            throw new Exceptions.DomainException(
                "Cannot change machine status from Alarm to Active. Use ClearAlarm method instead.");

        if (Status == newStatus)
            return; // No change needed

        var previousStatus = Status;
        Status = newStatus;
        SetUpdatedAudit(updatedBy);

        RaiseDomainEvent(new MachineStatusChangedEvent(
            Id,
            previousStatus,
            newStatus,
            reason));
    }

    /// <summary>
    /// Schedule maintenance for the machine
    /// </summary>
    public void ScheduleMaintenance(DateOnly date, MaintenanceInterval? interval = null, string? updatedBy = null)
    {
        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Maintenance date cannot be in the past", nameof(date));

        // Validation: NextMaintenance should be after LastMaintenance
        if (LastMaintenanceDate.HasValue && date <= LastMaintenanceDate.Value)
            throw new ArgumentException(
                "Next maintenance date must be after the last maintenance date",
                nameof(date));

        NextMaintenanceDate = date;
        MaintenanceIntervalId = interval?.Id;
        SetUpdatedAudit(updatedBy);

        RaiseDomainEvent(new MachineMaintenanceScheduledEvent(
            Id,
            date,
            interval?.Id));
    }

    /// <summary>
    /// Complete a maintenance operation
    /// </summary>
    public void CompleteMaintenance(DateOnly completedDate, MaintenanceInterval? interval = null, string? updatedBy = null)
    {
        if (completedDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("Completed date cannot be in the future", nameof(completedDate));

        LastMaintenanceDate = completedDate;

        // Automatically schedule next maintenance if interval is provided
        if (interval != null)
        {
            var nextDate = interval.CalculateNextDate(completedDate);
            NextMaintenanceDate = nextDate;
            MaintenanceIntervalId = interval.Id;

            RaiseDomainEvent(new MachineMaintenanceScheduledEvent(
                Id,
                nextDate,
                interval.Id));
        }
        else
        {
            NextMaintenanceDate = null;
            MaintenanceIntervalId = null;
        }

        SetUpdatedAudit(updatedBy);
    }

    /// <summary>
    /// Activate an alarm on the machine
    /// </summary>
    public void ActivateAlarm(string reason, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required for alarm activation", nameof(reason));

        if (Status == MachineStatus.Alarm)
            return; // Already in alarm state

        var previousStatus = Status;
        Status = MachineStatus.Alarm;
        SetUpdatedAudit(updatedBy);

        RaiseDomainEvent(new MachineAlarmActivatedEvent(
            Id,
            reason,
            previousStatus));
    }

    /// <summary>
    /// Clear an active alarm and restore previous operational status
    /// </summary>
    public void ClearAlarm(string reason, string? updatedBy = null)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Reason is required for alarm clearing", nameof(reason));

        if (Status != MachineStatus.Alarm)
            throw new Exceptions.DomainException("Cannot clear alarm: machine is not in alarm state");

        // Default to Active status when clearing alarm
        Status = MachineStatus.Active;
        SetUpdatedAudit(updatedBy);

        RaiseDomainEvent(new MachineAlarmClearedEvent(
            Id,
            reason));

        RaiseDomainEvent(new MachineStatusChangedEvent(
            Id,
            MachineStatus.Alarm,
            MachineStatus.Active,
            reason));
    }

    /// <summary>
    /// Update machine location
    /// </summary>
    public void UpdateLocation(string location, string? updatedBy = null)
    {
        if (location?.Length > 255)
            throw new ArgumentException("Location cannot exceed 255 characters", nameof(location));

        Location = location;
        SetUpdatedAudit(updatedBy);
    }
}
