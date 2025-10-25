using FluentAssertions;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Tests.Entities;

public class TicketTests
{
    private readonly Guid _organizationId = Guid.NewGuid();
    private readonly Guid _machineId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();
    private readonly TicketNumber _ticketNumber = TicketNumber.Create(2025, 1);

    [Fact]
    public void Create_WithValidData_ShouldCreateTicket()
    {
        // Arrange
        var title = "Machine malfunction";
        var description = "The machine stopped working";

        // Act
        var ticket = Ticket.Create(
            _ticketNumber,
            title,
            description,
            _organizationId,
            _machineId,
            Priority.High,
            _userId);

        // Assert
        ticket.Should().NotBeNull();
        ticket.Id.Should().NotBe(Guid.Empty);
        ticket.TicketNumber.Should().Be(_ticketNumber);
        ticket.Title.Should().Be(title);
        ticket.Description.Should().Be(description);
        ticket.OrganizationId.Should().Be(_organizationId);
        ticket.MachineId.Should().Be(_machineId);
        ticket.Priority.Should().Be(Priority.High);
        ticket.Status.Should().Be(TicketStatus.New);
        ticket.CreatedByUserId.Should().Be(_userId);
        ticket.AssignedToUserId.Should().BeNull();
        ticket.ResolvedAt.Should().BeNull();
        ticket.ClosedAt.Should().BeNull();
    }

    [Fact]
    public void Create_ShouldRaiseTicketCreatedEvent()
    {
        // Act
        var ticket = Ticket.Create(
            _ticketNumber,
            "Test ticket",
            "Description",
            _organizationId,
            _machineId,
            Priority.Medium,
            _userId);

        // Assert
        ticket.DomainEvents.Should().HaveCount(1);
        var domainEvent = ticket.DomainEvents.First();
        domainEvent.Should().BeOfType<TicketCreatedEvent>();

        var createdEvent = (TicketCreatedEvent)domainEvent;
        createdEvent.TicketId.Should().Be(ticket.Id);
        createdEvent.TicketNumber.Should().Be(_ticketNumber.Value);
        createdEvent.Title.Should().Be("Test ticket");
        createdEvent.OrganizationId.Should().Be(_organizationId);
        createdEvent.CreatedByUserId.Should().Be(_userId);
    }

    [Fact]
    public void Create_WithEmptyTitle_ShouldThrowException()
    {
        // Act
        Action act = () => Ticket.Create(
            _ticketNumber,
            "",
            "Description",
            _organizationId,
            _machineId,
            Priority.Low,
            _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot be empty*");
    }

    [Fact]
    public void Create_WithTitleTooLong_ShouldThrowException()
    {
        // Arrange
        var longTitle = new string('a', 256);

        // Act
        Action act = () => Ticket.Create(
            _ticketNumber,
            longTitle,
            "Description",
            _organizationId,
            _machineId,
            Priority.Low,
            _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Title cannot exceed 255 characters*");
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ShouldThrowException()
    {
        // Arrange
        var longDescription = new string('a', 5001);

        // Act
        Action act = () => Ticket.Create(
            _ticketNumber,
            "Valid title",
            longDescription,
            _organizationId,
            _machineId,
            Priority.Low,
            _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Description cannot exceed 5000 characters*");
    }

    [Fact]
    public void Create_WithNullTicketNumber_ShouldThrowException()
    {
        // Act
        Action act = () => Ticket.Create(
            null!,
            "Valid title",
            "Description",
            _organizationId,
            _machineId,
            Priority.Low,
            _userId);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void AssignTo_WithValidUserId_ShouldAssignTicket()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        var assignedToUserId = Guid.NewGuid();
        var assignedByUserId = Guid.NewGuid();

        // Act
        ticket.AssignTo(assignedToUserId, assignedByUserId);

        // Assert
        ticket.AssignedToUserId.Should().Be(assignedToUserId);
        ticket.DomainEvents.Should().Contain(e => e is TicketAssignedEvent);
        
        var assignedEvent = ticket.DomainEvents.OfType<TicketAssignedEvent>().First();
        assignedEvent.TicketId.Should().Be(ticket.Id);
        assignedEvent.AssignedToUserId.Should().Be(assignedToUserId);
        assignedEvent.AssignedByUserId.Should().Be(assignedByUserId);
    }

    [Fact]
    public void AssignTo_WithEmptyUserId_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket();

        // Act
        Action act = () => ticket.AssignTo(Guid.Empty, _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*User ID cannot be empty*");
    }

    [Fact]
    public void UpdateStatus_FromNewToInProgress_ShouldSucceed()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        var reason = "Starting work on the ticket";

        // Act
        ticket.UpdateStatus(TicketStatus.InProgress, reason, _userId);

        // Assert
        ticket.Status.Should().Be(TicketStatus.InProgress);
        ticket.DomainEvents.Should().Contain(e => e is TicketStatusChangedEvent);

        var statusChangedEvent = ticket.DomainEvents.OfType<TicketStatusChangedEvent>().Last();
        statusChangedEvent.OldStatus.Should().Be(TicketStatus.New);
        statusChangedEvent.NewStatus.Should().Be(TicketStatus.InProgress);
        statusChangedEvent.Reason.Should().Be(reason);
    }

    [Fact]
    public void UpdateStatus_WithInvalidTransition_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket(); // Status is New

        // Act - Try to close a new ticket (invalid transition)
        Action act = () => ticket.UpdateStatus(TicketStatus.Closed, "Trying to close", _userId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot transition from*");
    }

    [Fact]
    public void UpdateStatus_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket();

        // Act
        Action act = () => ticket.UpdateStatus(TicketStatus.InProgress, "", _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Reason for status change is required*");
    }

    [Fact]
    public void UpdateStatus_ToSameStatus_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket(); // Status is New

        // Act
        Action act = () => ticket.UpdateStatus(TicketStatus.New, "Same status", _userId);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Resolve_WithValidReason_ShouldResolveTicket()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        var reason = "Issue has been fixed";

        // Act
        ticket.Resolve(reason, _userId);

        // Assert
        ticket.Status.Should().Be(TicketStatus.Resolved);
        ticket.ResolvedAt.Should().NotBeNull();
        ticket.ResolvedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        
        ticket.DomainEvents.Should().Contain(e => e is TicketResolvedEvent);
        var resolvedEvent = ticket.DomainEvents.OfType<TicketResolvedEvent>().First();
        resolvedEvent.Reason.Should().Be(reason);
        resolvedEvent.ResolvedByUserId.Should().Be(_userId);
    }

    [Fact]
    public void Resolve_ClosedTicket_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.UpdateStatus(TicketStatus.InProgress, "Starting work", _userId);
        ticket.Resolve("Fixed", _userId);
        ticket.Close("Closing", _userId);

        // Act
        Action act = () => ticket.Resolve("Trying to resolve again", _userId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot resolve a closed ticket*");
    }

    [Fact]
    public void Resolve_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket();

        // Act
        Action act = () => ticket.Resolve("", _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Reason for resolution is required*");
    }

    [Fact]
    public void Close_ResolvedTicket_ShouldSucceed()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.Resolve("Issue fixed", _userId);
        var reason = "Confirmed by client";

        // Act
        ticket.Close(reason, _userId);

        // Assert
        ticket.Status.Should().Be(TicketStatus.Closed);
        ticket.ClosedAt.Should().NotBeNull();
        ticket.ClosedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        
        ticket.DomainEvents.Should().Contain(e => e is TicketClosedEvent);
        var closedEvent = ticket.DomainEvents.OfType<TicketClosedEvent>().First();
        closedEvent.Reason.Should().Be(reason);
        closedEvent.ClosedByUserId.Should().Be(_userId);
    }

    [Fact]
    public void Close_UnresolvedTicket_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket(); // Status is New

        // Act
        Action act = () => ticket.Close("Trying to close", _userId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Can only close a resolved ticket*");
    }

    [Fact]
    public void Close_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.Resolve("Fixed", _userId);

        // Act
        Action act = () => ticket.Close("", _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Reason for closing is required*");
    }

    [Fact]
    public void Reopen_ResolvedTicket_ShouldSucceed()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.Resolve("Fixed", _userId);
        var reason = "Issue persists";

        // Act
        ticket.Reopen(reason, _userId);

        // Assert
        ticket.Status.Should().Be(TicketStatus.Reopened);
        ticket.ResolvedAt.Should().BeNull();
        ticket.ClosedAt.Should().BeNull();
        
        var statusChangedEvents = ticket.DomainEvents.OfType<TicketStatusChangedEvent>();
        statusChangedEvents.Should().Contain(e => e.NewStatus == TicketStatus.Reopened);
    }

    [Fact]
    public void Reopen_ClosedTicket_ShouldSucceed()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.Resolve("Fixed", _userId);
        ticket.Close("Closed", _userId);
        var reason = "Issue came back";

        // Act
        ticket.Reopen(reason, _userId);

        // Assert
        ticket.Status.Should().Be(TicketStatus.Reopened);
        ticket.ResolvedAt.Should().BeNull();
        ticket.ClosedAt.Should().BeNull();
    }

    [Fact]
    public void Reopen_NewTicket_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket(); // Status is New

        // Act
        Action act = () => ticket.Reopen("Cannot reopen new ticket", _userId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Can only reopen resolved or closed tickets*");
    }

    [Fact]
    public void Reopen_WithEmptyReason_ShouldThrowException()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.Resolve("Fixed", _userId);

        // Act
        Action act = () => ticket.Reopen("", _userId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Reason for reopening is required*");
    }

    [Fact]
    public void StatusTransitions_ShouldFollowBusinessRules()
    {
        // Test valid transitions from New
        var ticket = CreateDefaultTicket();
        var act1 = () => ticket.UpdateStatus(TicketStatus.InProgress, "Starting", _userId);
        act1.Should().NotThrow();

        // Test valid transition from InProgress to Resolved
        ticket = CreateDefaultTicket();
        ticket.UpdateStatus(TicketStatus.InProgress, "Starting", _userId);
        var act2 = () => ticket.Resolve("Fixed", _userId);
        act2.Should().NotThrow();

        // Test valid transition from Resolved to Closed
        ticket = CreateDefaultTicket();
        ticket.Resolve("Fixed", _userId);
        var act3 = () => ticket.Close("Done", _userId);
        act3.Should().NotThrow();

        // Test valid transition from Closed to Reopened
        ticket = CreateDefaultTicket();
        ticket.Resolve("Fixed", _userId);
        ticket.Close("Done", _userId);
        var act4 = () => ticket.Reopen("Issue persists", _userId);
        act4.Should().NotThrow();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var ticket = CreateDefaultTicket();
        ticket.AssignTo(Guid.NewGuid(), _userId);
        ticket.DomainEvents.Should().HaveCountGreaterThan(0);

        // Act
        ticket.ClearDomainEvents();

        // Assert
        ticket.DomainEvents.Should().BeEmpty();
    }

    private Ticket CreateDefaultTicket()
    {
        return Ticket.Create(
            _ticketNumber,
            "Test ticket",
            "Test description",
            _organizationId,
            _machineId,
            Priority.Medium,
            _userId);
    }
}
