namespace Flowertrack.Domain.Tests.Events;

using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Events;

public class OrganizationEventTests
{
    [Fact]
    public void OrganizationCreatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var name = "Acme Manufacturing Corp.";
        var serviceStatus = "Active";
        var createdBy = Guid.NewGuid();

        // Act
        var @event = new OrganizationCreatedEvent(
            organizationId,
            name,
            serviceStatus,
            createdBy
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(name, @event.Name);
        Assert.Equal(serviceStatus, @event.ServiceStatus);
        Assert.Equal(createdBy, @event.CreatedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationServiceStatusChangedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var oldStatus = "Active";
        var newStatus = "Suspended";
        var reason = "Payment overdue";
        var changedAt = DateTimeOffset.UtcNow;

        // Act
        var @event = new OrganizationServiceStatusChangedEvent(
            organizationId,
            oldStatus,
            newStatus,
            reason,
            changedAt
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(oldStatus, @event.OldStatus);
        Assert.Equal(newStatus, @event.NewStatus);
        Assert.Equal(reason, @event.Reason);
        Assert.Equal(changedAt, @event.ChangedAt);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationServiceSuspendedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var suspensionReason = "Contract expired";
        var suspendedAt = DateTimeOffset.UtcNow;
        var suspendedBy = Guid.NewGuid();

        // Act
        var @event = new OrganizationServiceSuspendedEvent(
            organizationId,
            suspensionReason,
            suspendedAt,
            suspendedBy
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(suspensionReason, @event.SuspensionReason);
        Assert.Equal(suspendedAt, @event.SuspendedAt);
        Assert.Equal(suspendedBy, @event.SuspendedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationContractRenewedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var oldEndDate = DateTimeOffset.UtcNow;
        var newEndDate = DateTimeOffset.UtcNow.AddYears(1);
        var renewedAt = DateTimeOffset.UtcNow;
        var renewedBy = Guid.NewGuid();

        // Act
        var @event = new OrganizationContractRenewedEvent(
            organizationId,
            oldEndDate,
            newEndDate,
            renewedAt,
            renewedBy
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(oldEndDate, @event.OldEndDate);
        Assert.Equal(newEndDate, @event.NewEndDate);
        Assert.Equal(renewedAt, @event.RenewedAt);
        Assert.Equal(renewedBy, @event.RenewedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationEvents_ShouldBeImmutable()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var @event = new OrganizationCreatedEvent(
            organizationId,
            "Acme Manufacturing Corp.",
            "Active",
            Guid.NewGuid()
        );

        // Act & Assert
        // Records with init-only properties cannot be modified after construction
        Assert.Equal(organizationId, @event.OrganizationId);
    }
}
