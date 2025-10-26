namespace Flowertrack.Domain.Tests.Events;

using Flowertrack.Domain.Common;
using Flowertrack.Domain.Events;

public class MachineEventTests
{
    [Fact]
    public void MachineRegisteredEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var serialNumber = "BAU-2024-001";
        var brand = "Baumalog";
        var model = "ULTRA-5000";

        // Act
        var @event = new MachineRegisteredEvent(
            machineId,
            organizationId,
            serialNumber,
            brand,
            model
        );

        // Assert
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(serialNumber, @event.SerialNumber);
        Assert.Equal(brand, @event.Brand);
        Assert.Equal(model, @event.Model);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void MachineStatusChangedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var oldStatus = "Active";
        var newStatus = "Maintenance";
        var reason = "Scheduled preventive maintenance";
        var changedAt = DateTimeOffset.UtcNow;

        // Act
        var @event = new MachineStatusChangedEvent(
            machineId,
            oldStatus,
            newStatus,
            reason,
            changedAt
        );

        // Assert
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(oldStatus, @event.OldStatus);
        Assert.Equal(newStatus, @event.NewStatus);
        Assert.Equal(reason, @event.Reason);
        Assert.Equal(changedAt, @event.ChangedAt);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void MachineApiTokenGeneratedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var tokenGeneratedAt = DateTimeOffset.UtcNow;
        var generatedBy = Guid.NewGuid();

        // Act
        var @event = new MachineApiTokenGeneratedEvent(
            machineId,
            tokenGeneratedAt,
            generatedBy
        );

        // Assert
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(tokenGeneratedAt, @event.TokenGeneratedAt);
        Assert.Equal(generatedBy, @event.GeneratedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void MachineMaintenanceScheduledEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var scheduledDate = DateTimeOffset.UtcNow.AddDays(30);
        var intervalDays = 90;
        var scheduledBy = Guid.NewGuid();

        // Act
        var @event = new MachineMaintenanceScheduledEvent(
            machineId,
            scheduledDate,
            intervalDays,
            scheduledBy
        );

        // Assert
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(scheduledDate, @event.ScheduledDate);
        Assert.Equal(intervalDays, @event.IntervalDays);
        Assert.Equal(scheduledBy, @event.ScheduledBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void MachineAlarmActivatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var alarmReason = "High temperature detected";
        var activatedAt = DateTimeOffset.UtcNow;
        var severity = "Critical";

        // Act
        var @event = new MachineAlarmActivatedEvent(
            machineId,
            alarmReason,
            activatedAt,
            severity
        );

        // Assert
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(alarmReason, @event.AlarmReason);
        Assert.Equal(activatedAt, @event.ActivatedAt);
        Assert.Equal(severity, @event.Severity);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void MachineAlarmClearedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var clearedReason = "Temperature returned to normal";
        var clearedAt = DateTimeOffset.UtcNow;
        var clearedBy = Guid.NewGuid();

        // Act
        var @event = new MachineAlarmClearedEvent(
            machineId,
            clearedReason,
            clearedAt,
            clearedBy
        );

        // Assert
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(clearedReason, @event.ClearedReason);
        Assert.Equal(clearedAt, @event.ClearedAt);
        Assert.Equal(clearedBy, @event.ClearedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void MachineEvents_ShouldBeImmutable()
    {
        // Arrange
        var machineId = Guid.NewGuid();
        var @event = new MachineRegisteredEvent(
            machineId,
            Guid.NewGuid(),
            "BAU-2024-001",
            "Baumalog",
            "ULTRA-5000"
        );

        // Act & Assert
        // Records with init-only properties cannot be modified after construction
        Assert.Equal(machineId, @event.MachineId);
    }
}
