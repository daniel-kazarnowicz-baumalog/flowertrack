using Flowertrack.Domain.Common;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Events;
using Flowertrack.Domain.ValueObjects;

namespace Flowertrack.Domain.Entities;

/// <summary>
/// Represents a service ticket in the system (Aggregate Root)
/// </summary>
public sealed class Ticket : AuditableEntity<Guid>, IAggregateRoot
{
    private Ticket() { } // EF Core constructor

    // Private constructor for domain logic
    private Ticket(
        Guid id,
        TicketNumber ticketNumber,
        string title,
        string description,
        Guid organizationId,
        Guid machineId,
        Priority priority,
        Guid createdByUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be empty", nameof(title));
        }

        if (title.Length > 255)
        {
            throw new ArgumentException("Title cannot exceed 255 characters", nameof(title));
        }

        if (!string.IsNullOrEmpty(description) && description.Length > 5000)
        {
            throw new ArgumentException("Description cannot exceed 5000 characters", nameof(description));
        }

        Id = id;
        TicketNumber = ticketNumber ?? throw new ArgumentNullException(nameof(ticketNumber));
        Title = title;
        Description = description;
        OrganizationId = organizationId;
        MachineId = machineId;
        Priority = priority;
        Status = TicketStatus.New;
        CreatedByUserId = createdByUserId;
        SetCreatedAudit(createdByUserId);
    }

    public TicketNumber TicketNumber { get; private set; } = null!;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid OrganizationId { get; private set; }
    public Guid MachineId { get; private set; }
    public TicketStatus Status { get; private set; }
    public Priority Priority { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public DateTimeOffset? ResolvedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    /// <summary>
    /// Factory method to create a new ticket
    /// </summary>
    public static Ticket Create(
        TicketNumber ticketNumber,
        string title,
        string description,
        Guid organizationId,
        Guid machineId,
        Priority priority,
        Guid createdByUserId)
    {
        var ticket = new Ticket(
            Guid.NewGuid(),
            ticketNumber,
            title,
            description,
            organizationId,
            machineId,
            priority,
            createdByUserId);

        ticket.RaiseDomainEvent(new TicketCreatedEvent(
            ticket.Id,
            ticket.TicketNumber.Value,
            ticket.Title,
            ticket.OrganizationId,
            ticket.CreatedByUserId));

        return ticket;
    }

    /// <summary>
    /// Updates the ticket status with validation
    /// </summary>
    public void UpdateStatus(TicketStatus newStatus, string reason, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for status change is required", nameof(reason));
        }

        if (!IsValidStatusTransition(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Cannot transition from {Status} to {newStatus}");
        }

        var oldStatus = Status;
        Status = newStatus;
        SetUpdatedAudit(userId);

        RaiseDomainEvent(new TicketStatusChangedEvent(
            Id,
            oldStatus,
            newStatus,
            reason,
            userId));
    }

    /// <summary>
    /// Assigns the ticket to a user
    /// </summary>
    public void AssignTo(Guid userId, Guid assignedBy)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        }

        AssignedToUserId = userId;
        SetUpdatedAudit(assignedBy);

        RaiseDomainEvent(new TicketAssignedEvent(Id, userId, assignedBy));
    }

    /// <summary>
    /// Marks the ticket as resolved
    /// </summary>
    public void Resolve(string reason, Guid userId)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException("Cannot resolve a closed ticket");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for resolution is required", nameof(reason));
        }

        var oldStatus = Status;
        Status = TicketStatus.Resolved;
        ResolvedAt = DateTimeOffset.UtcNow;
        SetUpdatedAudit(userId);

        RaiseDomainEvent(new TicketStatusChangedEvent(
            Id,
            oldStatus,
            TicketStatus.Resolved,
            reason,
            userId));

        RaiseDomainEvent(new TicketResolvedEvent(
            Id,
            reason,
            userId,
            ResolvedAt.Value));
    }

    /// <summary>
    /// Closes the ticket
    /// </summary>
    public void Close(string reason, Guid userId)
    {
        if (Status != TicketStatus.Resolved)
        {
            throw new InvalidOperationException(
                "Can only close a resolved ticket. Current status: " + Status);
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for closing is required", nameof(reason));
        }

        var oldStatus = Status;
        Status = TicketStatus.Closed;
        ClosedAt = DateTimeOffset.UtcNow;
        SetUpdatedAudit(userId);

        RaiseDomainEvent(new TicketStatusChangedEvent(
            Id,
            oldStatus,
            TicketStatus.Closed,
            reason,
            userId));

        RaiseDomainEvent(new TicketClosedEvent(
            Id,
            reason,
            userId,
            ClosedAt.Value));
    }

    /// <summary>
    /// Reopens a previously resolved or closed ticket
    /// </summary>
    public void Reopen(string reason, Guid userId)
    {
        if (Status != TicketStatus.Resolved && Status != TicketStatus.Closed)
        {
            throw new InvalidOperationException(
                $"Can only reopen resolved or closed tickets. Current status: {Status}");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason for reopening is required", nameof(reason));
        }

        var oldStatus = Status;
        Status = TicketStatus.Reopened;
        ResolvedAt = null;
        ClosedAt = null;
        SetUpdatedAudit(userId);

        RaiseDomainEvent(new TicketStatusChangedEvent(
            Id,
            oldStatus,
            TicketStatus.Reopened,
            reason,
            userId));
    }

    /// <summary>
    /// Validates if a status transition is allowed
    /// </summary>
    private static bool IsValidStatusTransition(TicketStatus currentStatus, TicketStatus newStatus)
    {
        // Same status is not a valid transition
        if (currentStatus == newStatus)
        {
            return false;
        }

        return currentStatus switch
        {
            TicketStatus.New => newStatus is TicketStatus.InProgress or TicketStatus.Resolved,
            TicketStatus.InProgress => newStatus is TicketStatus.Resolved or TicketStatus.New,
            TicketStatus.Resolved => newStatus is TicketStatus.Closed or TicketStatus.Reopened,
            TicketStatus.Closed => newStatus == TicketStatus.Reopened,
            TicketStatus.Reopened => newStatus is TicketStatus.InProgress or TicketStatus.Resolved,
            _ => false
        };
    }
}
