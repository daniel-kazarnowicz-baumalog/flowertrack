namespace Flowertrack.Domain.ValueObjects;

/// <summary>
/// Ticket status in its lifecycle
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// Ticket in draft state (not sent yet)
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Ticket sent but not yet accepted by service team
    /// </summary>
    Sent = 1,

    /// <summary>
    /// Ticket accepted by service team
    /// </summary>
    Accepted = 2,

    /// <summary>
    /// Ticket is being worked on
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Ticket has been resolved
    /// </summary>
    Resolved = 4,

    /// <summary>
    /// Ticket reopened after being resolved
    /// </summary>
    Reopened = 5,

    /// <summary>
    /// Ticket closed (final state)
    /// </summary>
    Closed = 6,

    /// <summary>
    /// Ticket archived
    /// </summary>
    Archived = 7
}
