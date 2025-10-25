namespace Flowertrack.Domain.Enums;

/// <summary>
/// Represents the status of a ticket in the system
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket has been created and awaiting assignment or action
    /// </summary>
    New = 0,

    /// <summary>
    /// Ticket is being actively worked on by a technician
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Issue has been resolved but ticket is not yet closed
    /// </summary>
    Resolved = 2,

    /// <summary>
    /// Ticket has been closed and no further action is expected
    /// </summary>
    Closed = 3,

    /// <summary>
    /// Previously resolved ticket that has been reopened
    /// </summary>
    Reopened = 4
}
