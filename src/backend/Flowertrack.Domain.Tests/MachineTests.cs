using Flowertrack.Domain.Common;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests;

public class MachineTests
{
    private static readonly Guid TestOrganizationId = Guid.NewGuid();
    private const string TestSerialNumber = "TEST-MACHINE-001";
    private const string TestBrand = "TestBrand";
    private const string TestModel = "Model-X1";
    private const string TestLocation = "Factory Floor 1";

    [Fact]
    public void Create_WithValidParameters_CreatesNewMachine()
    {
        // Act
        var machine = Machine.Create(
            TestOrganizationId,
            TestSerialNumber,
            TestBrand,
            TestModel,
            TestLocation,
            "testuser");

        // Assert
        Assert.NotNull(machine);
        Assert.NotEqual(Guid.Empty, machine.Id);
        Assert.Equal(TestOrganizationId, machine.OrganizationId);
        Assert.Equal(TestSerialNumber, machine.SerialNumber);
        Assert.Equal(TestBrand, machine.Brand);
        Assert.Equal(TestModel, machine.Model);
        Assert.Equal(TestLocation, machine.Location);
        Assert.Equal(MachineStatus.Inactive, machine.Status);
        Assert.NotEqual(default, machine.CreatedAt);
        Assert.Equal("testuser", machine.CreatedBy);
    }

    [Fact]
    public void Create_WithValidParameters_RaisesMachineRegisteredEvent()
    {
        // Act
        var machine = Machine.Create(
            TestOrganizationId,
            TestSerialNumber,
            TestBrand,
            TestModel);

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var registeredEvent = Assert.IsType<MachineRegisteredEvent>(domainEvent);
        Assert.Equal(machine.Id, registeredEvent.MachineId);
        Assert.Equal(TestOrganizationId, registeredEvent.OrganizationId);
        Assert.Equal(TestSerialNumber, registeredEvent.SerialNumber);
        Assert.Equal(TestBrand, registeredEvent.Brand);
        Assert.Equal(TestModel, registeredEvent.Model);
    }

    [Fact]
    public void Create_WithEmptyOrganizationId_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(Guid.Empty, TestSerialNumber));

        Assert.Contains("Organization ID is required", exception.Message);
    }

    [Fact]
    public void Create_WithEmptySerialNumber_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(TestOrganizationId, ""));

        Assert.Contains("Serial number is required", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidSerialNumber_ThrowsArgumentException(string serialNumber)
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(TestOrganizationId, serialNumber));

        Assert.Contains("Serial number is required", exception.Message);
    }

    [Fact]
    public void Create_WithTooLongSerialNumber_ThrowsArgumentException()
    {
        // Arrange
        var longSerialNumber = new string('A', 256);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(TestOrganizationId, longSerialNumber));

        Assert.Contains("Serial number cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void Create_WithTooLongBrand_ThrowsArgumentException()
    {
        // Arrange
        var longBrand = new string('A', 101);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(TestOrganizationId, TestSerialNumber, longBrand));

        Assert.Contains("Brand cannot exceed 100 characters", exception.Message);
    }

    [Fact]
    public void Create_WithTooLongModel_ThrowsArgumentException()
    {
        // Arrange
        var longModel = new string('A', 101);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(TestOrganizationId, TestSerialNumber, TestBrand, longModel));

        Assert.Contains("Model cannot exceed 100 characters", exception.Message);
    }

    [Fact]
    public void Create_WithTooLongLocation_ThrowsArgumentException()
    {
        // Arrange
        var longLocation = new string('A', 256);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Machine.Create(TestOrganizationId, TestSerialNumber, TestBrand, TestModel, longLocation));

        Assert.Contains("Location cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void GenerateApiToken_CreatesNewToken()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ClearDomainEvents(); // Clear registration event

        // Act
        machine.GenerateApiToken("testuser");

        // Assert
        Assert.NotNull(machine.ApiToken);
        Assert.NotNull(machine.ApiToken.Value);
        Assert.True(machine.ApiToken.Value.Length >= 32);
        Assert.NotNull(machine.UpdatedBy);
        Assert.Equal("testuser", machine.UpdatedBy);
    }

    [Fact]
    public void GenerateApiToken_RaisesMachineApiTokenGeneratedEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ClearDomainEvents();

        // Act
        machine.GenerateApiToken();

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var tokenEvent = Assert.IsType<MachineApiTokenGeneratedEvent>(domainEvent);
        Assert.Equal(machine.Id, tokenEvent.MachineId);
        Assert.False(tokenEvent.IsRegeneration);
        Assert.Equal("Initial API token generation", tokenEvent.Reason);
    }

    [Fact]
    public void RegenerateApiToken_WithValidReason_CreatesNewToken()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.GenerateApiToken();
        var firstToken = machine.ApiToken;
        machine.ClearDomainEvents();

        // Act
        machine.RegenerateApiToken("Security breach", "testuser");

        // Assert
        Assert.NotNull(machine.ApiToken);
        Assert.NotEqual(firstToken?.Value, machine.ApiToken.Value);
    }

    [Fact]
    public void RegenerateApiToken_WithValidReason_RaisesRegenerationEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.GenerateApiToken();
        machine.ClearDomainEvents();

        // Act
        machine.RegenerateApiToken("Token compromised");

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var tokenEvent = Assert.IsType<MachineApiTokenGeneratedEvent>(domainEvent);
        Assert.Equal(machine.Id, tokenEvent.MachineId);
        Assert.True(tokenEvent.IsRegeneration);
        Assert.Equal("Token compromised", tokenEvent.Reason);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RegenerateApiToken_WithInvalidReason_ThrowsArgumentException(string reason)
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.GenerateApiToken();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.RegenerateApiToken(reason));

        Assert.Contains("Reason is required for token regeneration", exception.Message);
    }

    [Fact]
    public void UpdateStatus_WithValidParameters_UpdatesStatus()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ClearDomainEvents();

        // Act
        machine.UpdateStatus(MachineStatus.Active, "Initial activation", "testuser");

        // Assert
        Assert.Equal(MachineStatus.Active, machine.Status);
        Assert.NotNull(machine.UpdatedBy);
        Assert.Equal("testuser", machine.UpdatedBy);
    }

    [Fact]
    public void UpdateStatus_WithValidParameters_RaisesStatusChangedEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ClearDomainEvents();

        // Act
        machine.UpdateStatus(MachineStatus.Active, "Activation");

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var statusEvent = Assert.IsType<MachineStatusChangedEvent>(domainEvent);
        Assert.Equal(machine.Id, statusEvent.MachineId);
        Assert.Equal(MachineStatus.Inactive, statusEvent.PreviousStatus);
        Assert.Equal(MachineStatus.Active, statusEvent.NewStatus);
        Assert.Equal("Activation", statusEvent.Reason);
    }

    [Fact]
    public void UpdateStatus_WithSameStatus_DoesNotRaiseEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ClearDomainEvents();

        // Act
        machine.UpdateStatus(MachineStatus.Inactive, "No change");

        // Assert
        Assert.Empty(machine.DomainEvents);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateStatus_WithInvalidReason_ThrowsArgumentException(string reason)
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.UpdateStatus(MachineStatus.Active, reason));

        Assert.Contains("Reason is required for status change", exception.Message);
    }

    [Fact]
    public void UpdateStatus_FromAlarmToActive_ThrowsDomainException()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ActivateAlarm("Test alarm");

        // Act & Assert
        var exception = Assert.Throws<Flowertrack.Domain.Exceptions.DomainException>(() =>
            machine.UpdateStatus(MachineStatus.Active, "Trying to activate"));

        Assert.Contains("Cannot change machine status from Alarm to Active", exception.Message);
    }

    [Fact]
    public void ScheduleMaintenance_WithValidDate_UpdatesMaintenanceDates()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var interval = MaintenanceInterval.Create(1, 90, "Quarterly Maintenance");
        machine.ClearDomainEvents();

        // Act
        machine.ScheduleMaintenance(futureDate, interval, "testuser");

        // Assert
        Assert.Equal(futureDate, machine.NextMaintenanceDate);
        Assert.Equal(interval.Id, machine.MaintenanceIntervalId);
    }

    [Fact]
    public void ScheduleMaintenance_WithValidDate_RaisesMaintenanceScheduledEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        machine.ClearDomainEvents();

        // Act
        machine.ScheduleMaintenance(futureDate);

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var maintenanceEvent = Assert.IsType<MachineMaintenanceScheduledEvent>(domainEvent);
        Assert.Equal(machine.Id, maintenanceEvent.MachineId);
        Assert.Equal(futureDate, maintenanceEvent.ScheduledDate);
    }

    [Fact]
    public void ScheduleMaintenance_WithPastDate_ThrowsArgumentException()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var pastDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.ScheduleMaintenance(pastDate));

        Assert.Contains("Maintenance date cannot be in the past", exception.Message);
    }

    [Fact]
    public void ScheduleMaintenance_WithDateEqualToLastMaintenance_ThrowsArgumentException()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var futureDate = today.AddDays(30);
        machine.CompleteMaintenance(today);

        // Try to schedule maintenance for today (equal to last maintenance)
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.ScheduleMaintenance(today));

        Assert.Contains("Next maintenance date must be after the last maintenance date", exception.Message);
    }

    [Fact]
    public void CompleteMaintenance_WithValidDate_UpdatesMaintenanceDates()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var completedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var interval = MaintenanceInterval.Create(1, 90, "Quarterly");
        machine.ClearDomainEvents();

        // Act
        machine.CompleteMaintenance(completedDate, interval, "testuser");

        // Assert
        Assert.Equal(completedDate, machine.LastMaintenanceDate);
        Assert.NotNull(machine.NextMaintenanceDate);
        Assert.Equal(completedDate.AddDays(90), machine.NextMaintenanceDate);
    }

    [Fact]
    public void CompleteMaintenance_WithInterval_AutomaticallySchedulesNext()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var completedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        var interval = MaintenanceInterval.Create(1, 90, "Quarterly");
        machine.ClearDomainEvents();

        // Act
        machine.CompleteMaintenance(completedDate, interval);

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var maintenanceEvent = Assert.IsType<MachineMaintenanceScheduledEvent>(domainEvent);
        Assert.Equal(completedDate.AddDays(90), maintenanceEvent.ScheduledDate);
    }

    [Fact]
    public void CompleteMaintenance_WithFutureDate_ThrowsArgumentException()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var futureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.CompleteMaintenance(futureDate));

        Assert.Contains("Completed date cannot be in the future", exception.Message);
    }

    [Fact]
    public void ActivateAlarm_WithValidReason_ChangesStatusToAlarm()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.UpdateStatus(MachineStatus.Active, "Activation");
        machine.ClearDomainEvents();

        // Act
        machine.ActivateAlarm("Critical error detected", "testuser");

        // Assert
        Assert.Equal(MachineStatus.Alarm, machine.Status);
    }

    [Fact]
    public void ActivateAlarm_WithValidReason_RaisesAlarmActivatedEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.UpdateStatus(MachineStatus.Active, "Activation");
        machine.ClearDomainEvents();

        // Act
        machine.ActivateAlarm("Critical error");

        // Assert
        var domainEvent = Assert.Single(machine.DomainEvents);
        var alarmEvent = Assert.IsType<MachineAlarmActivatedEvent>(domainEvent);
        Assert.Equal(machine.Id, alarmEvent.MachineId);
        Assert.Equal("Critical error", alarmEvent.Reason);
        Assert.Equal(MachineStatus.Active, alarmEvent.PreviousStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ActivateAlarm_WithInvalidReason_ThrowsArgumentException(string reason)
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.ActivateAlarm(reason));

        Assert.Contains("Reason is required for alarm activation", exception.Message);
    }

    [Fact]
    public void ActivateAlarm_WhenAlreadyInAlarmState_DoesNotRaiseEvent()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ActivateAlarm("First alarm");
        machine.ClearDomainEvents();

        // Act
        machine.ActivateAlarm("Second alarm");

        // Assert
        Assert.Empty(machine.DomainEvents);
    }

    [Fact]
    public void ClearAlarm_WithValidReason_ChangesStatusToActive()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ActivateAlarm("Test alarm");
        machine.ClearDomainEvents();

        // Act
        machine.ClearAlarm("Issue resolved", "testuser");

        // Assert
        Assert.Equal(MachineStatus.Active, machine.Status);
    }

    [Fact]
    public void ClearAlarm_WithValidReason_RaisesBothAlarmClearedAndStatusChangedEvents()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ActivateAlarm("Test alarm");
        machine.ClearDomainEvents();

        // Act
        machine.ClearAlarm("Issue resolved");

        // Assert
        Assert.Equal(2, machine.DomainEvents.Count);
        Assert.Contains(machine.DomainEvents, e => e is MachineAlarmClearedEvent);
        Assert.Contains(machine.DomainEvents, e => e is MachineStatusChangedEvent);
    }

    [Fact]
    public void ClearAlarm_WhenNotInAlarmState_ThrowsDomainException()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);

        // Act & Assert
        var exception = Assert.Throws<Flowertrack.Domain.Exceptions.DomainException>(() =>
            machine.ClearAlarm("Trying to clear"));

        Assert.Contains("Cannot clear alarm: machine is not in alarm state", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ClearAlarm_WithInvalidReason_ThrowsArgumentException(string reason)
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        machine.ActivateAlarm("Test alarm");

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.ClearAlarm(reason));

        Assert.Contains("Reason is required for alarm clearing", exception.Message);
    }

    [Fact]
    public void UpdateLocation_WithValidLocation_UpdatesLocation()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var newLocation = "New Factory Location";

        // Act
        machine.UpdateLocation(newLocation, "testuser");

        // Assert
        Assert.Equal(newLocation, machine.Location);
        Assert.Equal("testuser", machine.UpdatedBy);
    }

    [Fact]
    public void UpdateLocation_WithTooLongLocation_ThrowsArgumentException()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        var longLocation = new string('A', 256);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            machine.UpdateLocation(longLocation));

        Assert.Contains("Location cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        // Arrange
        var machine = Machine.Create(TestOrganizationId, TestSerialNumber);
        Assert.NotEmpty(machine.DomainEvents);

        // Act
        machine.ClearDomainEvents();

        // Assert
        Assert.Empty(machine.DomainEvents);
    }
}
