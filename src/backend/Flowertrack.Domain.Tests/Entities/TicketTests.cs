using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests.Entities;

/// <summary>
/// Unit tests for Ticket entity
/// </summary>
public class TicketTests
{
    private static TicketNumber CreateTestTicketNumber() => TicketNumber.Create(2025, 1);
    private static Guid TestOrganizationId => Guid.NewGuid();
    private static Guid TestMachineId => Guid.NewGuid();
    private static Guid TestUserId => Guid.NewGuid();

    #region Factory Method Tests

    [Fact]
    public void Create_WithValidData_ShouldCreateTicket()
    {
        // Arrange
        var ticketNumber = CreateTestTicketNumber();
        var title = "Test Ticket";
        var description = "Test Description";
        var orgId = TestOrganizationId;
        var machineId = TestMachineId;
        var userId = TestUserId;
        var priority = Priority.High;

        // Act
        var ticket = Ticket.Create(
            ticketNumber,
            title,
            description,
            orgId,
            machineId,
            priority,
            userId);

        // Assert
        Assert.NotNull(ticket);
        Assert.Equal(ticketNumber, ticket.TicketNumber);
        Assert.Equal(title, ticket.Title);
        Assert.Equal(description, ticket.Description);
        Assert.Equal(orgId, ticket.OrganizationId);
        Assert.Equal(machineId, ticket.MachineId);
        Assert.Equal(priority, ticket.Priority);
        Assert.Equal(TicketStatus.New, ticket.Status);
        Assert.Equal(userId, ticket.CreatedByUserId);
        Assert.Null(ticket.AssignedToUserId);
        Assert.Null(ticket.ResolvedAt);
        Assert.Null(ticket.ClosedAt);
    }

    [Fact]
    public void Create_WithValidData_ShouldRaiseTicketCreatedEvent()
    {
        // Arrange
        var ticketNumber = CreateTestTicketNumber();
        var title = "Test Ticket";
        var description = "Test Description";

        // Act
        var ticket = Ticket.Create(
            ticketNumber,
            title,
            description,
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Assert
        var domainEvents = ticket.DomainEvents.ToList();
        Assert.Single(domainEvents);
        var createdEvent = Assert.IsType<TicketCreatedEvent>(domainEvents[0]);
        Assert.Equal(ticket.Id, createdEvent.TicketId);
        Assert.Equal(ticketNumber.Value, createdEvent.TicketNumber);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowArgumentException()
    {
        // Arrange
        var ticketNumber = CreateTestTicketNumber();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Ticket.Create(
                ticketNumber,
                "",
                "Description",
                TestOrganizationId,
                TestMachineId,
                Priority.Low,
                TestUserId));

        Assert.Contains("Title cannot be empty", exception.Message);
    }

    [Fact]
    public void Create_WithTitleTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var ticketNumber = CreateTestTicketNumber();
        var longTitle = new string('A', 256);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Ticket.Create(
                ticketNumber,
                longTitle,
                "Description",
                TestOrganizationId,
                TestMachineId,
                Priority.Low,
                TestUserId));

        Assert.Contains("Title cannot exceed 255 characters", exception.Message);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldThrowArgumentException()
    {
        // Arrange
        var ticketNumber = CreateTestTicketNumber();
        var longDescription = new string('A', 5001);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            Ticket.Create(
                ticketNumber,
                "Title",
                longDescription,
                TestOrganizationId,
                TestMachineId,
                Priority.Low,
                TestUserId));

        Assert.Contains("Description cannot exceed 5000 characters", exception.Message);
    }

    [Fact]
    public void Create_WithNullTicketNumber_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            Ticket.Create(
                null!,
                "Title",
                "Description",
                TestOrganizationId,
                TestMachineId,
                Priority.Low,
                TestUserId));
    }

    #endregion

    #region UpdateStatus Tests

    [Fact]
    public void UpdateStatus_FromNewToInProgress_ShouldSucceed()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();

        // Act
        ticket.UpdateStatus(TicketStatus.InProgress, "Starting work", userId);

        // Assert
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Fact]
    public void UpdateStatus_WithValidTransition_ShouldRaiseStatusChangedEvent()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        ticket.ClearDomainEvents(); // Clear creation event
        var userId = Guid.NewGuid();

        // Act
        ticket.UpdateStatus(TicketStatus.InProgress, "Starting work", userId);

        // Assert
        var domainEvents = ticket.DomainEvents.ToList();
        Assert.Single(domainEvents);
        var statusChangedEvent = Assert.IsType<TicketStatusChangedEvent>(domainEvents[0]);
        Assert.Equal(TicketStatus.New.ToString(), statusChangedEvent.OldStatus);
        Assert.Equal(TicketStatus.InProgress.ToString(), statusChangedEvent.NewStatus);
    }

    [Fact]
    public void UpdateStatus_WithInvalidTransition_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ticket.UpdateStatus(TicketStatus.Closed, "Invalid", Guid.NewGuid()));

        Assert.Contains("Cannot transition from", exception.Message);
    }

    [Fact]
    public void UpdateStatus_WithSameStatus_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            ticket.UpdateStatus(TicketStatus.New, "Same status", Guid.NewGuid()));
    }

    [Fact]
    public void UpdateStatus_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ticket.UpdateStatus(TicketStatus.InProgress, "", Guid.NewGuid()));

        Assert.Contains("Reason for status change is required", exception.Message);
    }

    #endregion

    #region AssignTo Tests

    [Fact]
    public void AssignTo_WithValidUserId_ShouldAssignTicket()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var assigneeId = Guid.NewGuid();
        var assignedBy = Guid.NewGuid();

        // Act
        ticket.AssignTo(assigneeId, assignedBy);

        // Assert
        Assert.Equal(assigneeId, ticket.AssignedToUserId);
    }

    [Fact]
    public void AssignTo_WithValidUserId_ShouldRaiseTicketAssignedEvent()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        ticket.ClearDomainEvents();
        var assigneeId = Guid.NewGuid();
        var assignedBy = Guid.NewGuid();

        // Act
        ticket.AssignTo(assigneeId, assignedBy);

        // Assert
        var domainEvents = ticket.DomainEvents.ToList();
        Assert.Single(domainEvents);
        var assignedEvent = Assert.IsType<TicketAssignedEvent>(domainEvents[0]);
        Assert.Equal(ticket.Id, assignedEvent.TicketId);
        Assert.Equal(assigneeId, assignedEvent.AssignedToUserId);
    }

    [Fact]
    public void AssignTo_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ticket.AssignTo(Guid.Empty, Guid.NewGuid()));

        Assert.Contains("User ID cannot be empty", exception.Message);
    }

    #endregion

    #region Resolve Tests

    [Fact]
    public void Resolve_FromValidStatus_ShouldResolveTicket()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();

        // Act
        ticket.Resolve("Issue fixed", userId);

        // Assert
        Assert.Equal(TicketStatus.Resolved, ticket.Status);
        Assert.NotNull(ticket.ResolvedAt);
        Assert.True(ticket.ResolvedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Resolve_ShouldRaiseResolvedEvent()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        ticket.ClearDomainEvents();
        var userId = Guid.NewGuid();

        // Act
        ticket.Resolve("Issue fixed", userId);

        // Assert
        var domainEvents = ticket.DomainEvents.ToList();
        Assert.Contains(domainEvents, e => e is TicketResolvedEvent);
    }

    [Fact]
    public void Resolve_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ticket.Resolve("", Guid.NewGuid()));

        Assert.Contains("Reason for resolution is required", exception.Message);
    }

    [Fact]
    public void Resolve_WhenClosed_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();
        ticket.Resolve("Fixed", userId);
        ticket.Close("Verified", userId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ticket.Resolve("Try again", userId));

        Assert.Contains("Cannot resolve a closed ticket", exception.Message);
    }

    #endregion

    #region Close Tests

    [Fact]
    public void Close_WhenResolved_ShouldCloseTicket()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();
        ticket.Resolve("Issue fixed", userId);

        // Act
        ticket.Close("Verified by customer", userId);

        // Assert
        Assert.Equal(TicketStatus.Closed, ticket.Status);
        Assert.NotNull(ticket.ClosedAt);
        Assert.True(ticket.ClosedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Close_ShouldRaiseClosedEvent()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();
        ticket.Resolve("Issue fixed", userId);
        ticket.ClearDomainEvents();

        // Act
        ticket.Close("Verified", userId);

        // Assert
        var domainEvents = ticket.DomainEvents.ToList();
        Assert.Contains(domainEvents, e => e is TicketClosedEvent);
    }

    [Fact]
    public void Close_WhenNotResolved_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ticket.Close("Trying to close", Guid.NewGuid()));

        Assert.Contains("Can only close a resolved ticket", exception.Message);
    }

    [Fact]
    public void Close_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        ticket.Resolve("Fixed", Guid.NewGuid());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ticket.Close("", Guid.NewGuid()));

        Assert.Contains("Reason for closing is required", exception.Message);
    }

    #endregion

    #region Reopen Tests

    [Fact]
    public void Reopen_WhenResolved_ShouldReopenTicket()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();
        ticket.Resolve("Issue fixed", userId);

        // Act
        ticket.Reopen("Issue not actually fixed", userId);

        // Assert
        Assert.Equal(TicketStatus.Reopened, ticket.Status);
        Assert.Null(ticket.ResolvedAt);
        Assert.Null(ticket.ClosedAt);
    }

    [Fact]
    public void Reopen_WhenClosed_ShouldReopenTicket()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        var userId = Guid.NewGuid();
        ticket.Resolve("Issue fixed", userId);
        ticket.Close("Verified", userId);

        // Act
        ticket.Reopen("Problem reoccurred", userId);

        // Assert
        Assert.Equal(TicketStatus.Reopened, ticket.Status);
        Assert.Null(ticket.ResolvedAt);
        Assert.Null(ticket.ClosedAt);
    }

    [Fact]
    public void Reopen_WhenNew_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            ticket.Reopen("Cannot reopen", Guid.NewGuid()));

        Assert.Contains("Can only reopen resolved or closed tickets", exception.Message);
    }

    [Fact]
    public void Reopen_WithEmptyReason_ShouldThrowArgumentException()
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);
        ticket.Resolve("Fixed", Guid.NewGuid());

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() =>
            ticket.Reopen("", Guid.NewGuid()));

        Assert.Contains("Reason for reopening is required", exception.Message);
    }

    #endregion

    #region Status Transition Tests

    [Theory]
    [InlineData(TicketStatus.New, TicketStatus.InProgress)]
    [InlineData(TicketStatus.New, TicketStatus.Resolved)]
    [InlineData(TicketStatus.InProgress, TicketStatus.Resolved)]
    [InlineData(TicketStatus.InProgress, TicketStatus.New)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Closed)]
    [InlineData(TicketStatus.Resolved, TicketStatus.Reopened)]
    [InlineData(TicketStatus.Closed, TicketStatus.Reopened)]
    [InlineData(TicketStatus.Reopened, TicketStatus.InProgress)]
    [InlineData(TicketStatus.Reopened, TicketStatus.Resolved)]
    public void UpdateStatus_ValidTransitions_ShouldSucceed(TicketStatus fromStatus, TicketStatus toStatus)
    {
        // Arrange
        var ticket = Ticket.Create(
            CreateTestTicketNumber(),
            "Test Ticket",
            "Description",
            TestOrganizationId,
            TestMachineId,
            Priority.Medium,
            TestUserId);

        // Set ticket to fromStatus
        SetTicketStatus(ticket, fromStatus);

        // Act & Assert (should not throw)
        ticket.UpdateStatus(toStatus, "Valid transition", Guid.NewGuid());
        Assert.Equal(toStatus, ticket.Status);
    }

    private static void SetTicketStatus(Ticket ticket, TicketStatus status)
    {
        var userId = Guid.NewGuid();
        switch (status)
        {
            case TicketStatus.New:
                // Already New
                break;
            case TicketStatus.InProgress:
                ticket.UpdateStatus(TicketStatus.InProgress, "Set to InProgress", userId);
                break;
            case TicketStatus.Resolved:
                ticket.Resolve("Resolved", userId);
                break;
            case TicketStatus.Closed:
                ticket.Resolve("Resolved", userId);
                ticket.Close("Closed", userId);
                break;
            case TicketStatus.Reopened:
                ticket.Resolve("Resolved", userId);
                ticket.Reopen("Reopened", userId);
                break;
        }
    }

    #endregion
}
