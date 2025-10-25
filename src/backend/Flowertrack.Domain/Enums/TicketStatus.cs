namespace Flowertrack.Domain.Enums;

/// <summary>
/// Represents the status of a ticket in the system.
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket has been created but not yet submitted.
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Ticket has been submitted and is waiting for assignment.
    /// </summary>
    New = 1,

    /// <summary>
    /// Ticket has been assigned to a service technician.
    /// </summary>
    Assigned = 2,

    /// <summary>
    /// Technician is actively working on the ticket.
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Ticket has been resolved but not yet closed.
    /// </summary>
    Resolved = 4,

    /// <summary>
    /// Ticket has been closed.
    /// </summary>
    Closed = 5,

    /// <summary>
    /// Ticket has been reopened after being resolved.
    /// </summary>
    Reopened = 6
}
