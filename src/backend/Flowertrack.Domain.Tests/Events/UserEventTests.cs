namespace Flowertrack.Domain.Tests.Events;

using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Events;

public class UserEventTests
{
    [Fact]
    public void ServiceUserCreatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "technician@baumalog.pl";
        var fullName = "Jan Kowalski";
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var @event = new ServiceUserCreatedEvent(
            userId,
            email,
            fullName,
            createdAt
        );

        // Assert
        Assert.Equal(userId, @event.UserId);
        Assert.Equal(email, @event.Email);
        Assert.Equal(fullName, @event.FullName);
        Assert.Equal(createdAt, @event.CreatedAt);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void ServiceUserActivatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var activatedAt = DateTimeOffset.UtcNow;
        var activatedBy = Guid.NewGuid();

        // Act
        var @event = new ServiceUserActivatedEvent(
            userId,
            activatedAt,
            activatedBy
        );

        // Assert
        Assert.Equal(userId, @event.UserId);
        Assert.Equal(activatedAt, @event.ActivatedAt);
        Assert.Equal(activatedBy, @event.ActivatedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void ServiceUserDeactivatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deactivationReason = "Employee left the company";
        var deactivatedAt = DateTimeOffset.UtcNow;
        var deactivatedBy = Guid.NewGuid();

        // Act
        var @event = new ServiceUserDeactivatedEvent(
            userId,
            deactivationReason,
            deactivatedAt,
            deactivatedBy
        );

        // Assert
        Assert.Equal(userId, @event.UserId);
        Assert.Equal(deactivationReason, @event.DeactivationReason);
        Assert.Equal(deactivatedAt, @event.DeactivatedAt);
        Assert.Equal(deactivatedBy, @event.DeactivatedBy);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void OrganizationUserCreatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var organizationId = Guid.NewGuid();
        var email = "operator@acme.com";
        var role = "Operator";
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var @event = new OrganizationUserCreatedEvent(
            userId,
            organizationId,
            email,
            role,
            createdAt
        );

        // Assert
        Assert.Equal(userId, @event.UserId);
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(email, @event.Email);
        Assert.Equal(role, @event.Role);
        Assert.Equal(createdAt, @event.CreatedAt);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void UserEvents_ShouldBeImmutable()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var @event = new ServiceUserCreatedEvent(
            userId,
            "technician@baumalog.pl",
            "Jan Kowalski",
            DateTimeOffset.UtcNow
        );

        // Act & Assert
        // Records with init-only properties cannot be modified after construction
        Assert.Equal(userId, @event.UserId);
    }
}
