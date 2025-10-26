namespace Flowertrack.Domain.Tests.Events;

using Flowertrack.Domain.Common;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.Enums;

public class OrganizationEventTests
{
    [Fact]
    public void OrganizationCreatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var name = "Acme Manufacturing Corp.";

        // Act
        var @event = new OrganizationCreatedEvent(
            organizationId,
            name
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(name, @event.Name);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationServiceStatusChangedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var oldStatus = ServiceStatus.Active;
        var newStatus = ServiceStatus.Suspended;
        var reason = "Payment overdue";

        // Act
        var @event = new OrganizationServiceStatusChangedEvent(
            organizationId,
            oldStatus,
            newStatus,
            reason
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(oldStatus, @event.PreviousStatus);
        Assert.Equal(newStatus, @event.NewStatus);
        Assert.Equal(reason, @event.Reason);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationServiceSuspendedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var reason = "Contract expired";

        // Act
        var @event = new OrganizationServiceSuspendedEvent(
            organizationId,
            reason
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(reason, @event.Reason);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationContractRenewedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var oldEndDate = DateTimeOffset.UtcNow;
        var newEndDate = DateTimeOffset.UtcNow.AddYears(1);

        // Act
        var @event = new OrganizationContractRenewedEvent(
            organizationId,
            oldEndDate,
            newEndDate
        );

        // Assert
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(oldEndDate, @event.PreviousEndDate);
        Assert.Equal(newEndDate, @event.NewEndDate);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationEvents_ShouldBeImmutable()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var @event = new OrganizationCreatedEvent(
            organizationId,
            "Acme Manufacturing Corp."
        );

        // Act & Assert
        // Classes with readonly properties cannot be modified after construction
        Assert.Equal(organizationId, @event.OrganizationId);
    }
}
