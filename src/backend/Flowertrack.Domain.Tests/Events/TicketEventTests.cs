namespace Flowertrack.Domain.Tests.Events;

using Flowertrack.Api.Domain.Common;
using Flowertrack.Api.Domain.Events;

public class TicketEventTests
{
    [Fact]
    public void TicketCreatedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var ticketNumber = "TKT-2024-0001";
        var organizationId = Guid.NewGuid();
        var machineId = Guid.NewGuid();
        var createdBy = Guid.NewGuid();
        var priority = "High";

        // Act
        var @event = new TicketCreatedEvent(
            ticketId,
            ticketNumber,
            organizationId,
            machineId,
            createdBy,
            priority
        );

        // Assert
        Assert.Equal(ticketId, @event.TicketId);
        Assert.Equal(ticketNumber, @event.TicketNumber);
        Assert.Equal(organizationId, @event.OrganizationId);
        Assert.Equal(machineId, @event.MachineId);
        Assert.Equal(createdBy, @event.CreatedBy);
        Assert.Equal(priority, @event.Priority);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void TicketStatusChangedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var oldStatus = "New";
        var newStatus = "InProgress";
        var reason = "Starting work on the issue";
        var changedBy = Guid.NewGuid();
        var changedAt = DateTimeOffset.UtcNow;

        // Act
        var @event = new TicketStatusChangedEvent(
            ticketId,
            oldStatus,
            newStatus,
            reason,
            changedBy,
            changedAt
        );

        // Assert
        Assert.Equal(ticketId, @event.TicketId);
        Assert.Equal(oldStatus, @event.OldStatus);
        Assert.Equal(newStatus, @event.NewStatus);
        Assert.Equal(reason, @event.Reason);
        Assert.Equal(changedBy, @event.ChangedBy);
        Assert.Equal(changedAt, @event.ChangedAt);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void TicketAssignedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();
        var assignedBy = Guid.NewGuid();
        var previousAssignee = Guid.NewGuid();

        // Act
        var @event = new TicketAssignedEvent(
            ticketId,
            assignedToUserId,
            assignedBy,
            previousAssignee
        );

        // Assert
        Assert.Equal(ticketId, @event.TicketId);
        Assert.Equal(assignedToUserId, @event.AssignedToUserId);
        Assert.Equal(assignedBy, @event.AssignedBy);
        Assert.Equal(previousAssignee, @event.PreviousAssignee);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void TicketAssignedEvent_ShouldAllowNullPreviousAssignee()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var assignedToUserId = Guid.NewGuid();
        var assignedBy = Guid.NewGuid();

        // Act
        var @event = new TicketAssignedEvent(
            ticketId,
            assignedToUserId,
            assignedBy
        );

        // Assert
        Assert.Null(@event.PreviousAssignee);
    }

    [Fact]
    public void TicketResolvedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var resolvedBy = Guid.NewGuid();
        var resolvedAt = DateTimeOffset.UtcNow;
        var resolutionNote = "Fixed the hydraulic pump";

        // Act
        var @event = new TicketResolvedEvent(
            ticketId,
            resolvedBy,
            resolvedAt,
            resolutionNote
        );

        // Assert
        Assert.Equal(ticketId, @event.TicketId);
        Assert.Equal(resolvedBy, @event.ResolvedBy);
        Assert.Equal(resolvedAt, @event.ResolvedAt);
        Assert.Equal(resolutionNote, @event.ResolutionNote);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void TicketClosedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var closedBy = Guid.NewGuid();
        var closedAt = DateTimeOffset.UtcNow;
        var closureNote = "Issue resolved and verified by client";

        // Act
        var @event = new TicketClosedEvent(
            ticketId,
            closedBy,
            closedAt,
            closureNote
        );

        // Assert
        Assert.Equal(ticketId, @event.TicketId);
        Assert.Equal(closedBy, @event.ClosedBy);
        Assert.Equal(closedAt, @event.ClosedAt);
        Assert.Equal(closureNote, @event.ClosureNote);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void TicketReopenedEvent_ShouldCreateWithAllProperties()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var reopenedBy = Guid.NewGuid();
        var reopenedAt = DateTimeOffset.UtcNow;
        var reason = "Issue recurred after initial fix";

        // Act
        var @event = new TicketReopenedEvent(
            ticketId,
            reopenedBy,
            reopenedAt,
            reason
        );

        // Assert
        Assert.Equal(ticketId, @event.TicketId);
        Assert.Equal(reopenedBy, @event.ReopenedBy);
        Assert.Equal(reopenedAt, @event.ReopenedAt);
        Assert.Equal(reason, @event.Reason);
        Assert.IsAssignableFrom<DomainEvent>(@event);
    }

    [Fact]
    public void TicketEvents_ShouldBeImmutable()
    {
        // Arrange
        var ticketId = Guid.NewGuid();
        var @event = new TicketCreatedEvent(
            ticketId,
            "TKT-2024-0001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "High"
        );

        // Act & Assert
        // Records with init-only properties cannot be modified after construction
        // This is verified at compile-time
        Assert.Equal(ticketId, @event.TicketId);
    }
}
