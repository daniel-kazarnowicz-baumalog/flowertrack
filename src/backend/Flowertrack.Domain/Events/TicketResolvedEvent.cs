using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a ticket is marked as resolved.
/// This indicates that the issue has been fixed and is awaiting client verification or closure.
/// </summary>
public sealed class TicketResolvedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the ticket
    /// </summary>
    public Guid TicketId { get; }

    /// <summary>
    /// User who resolved the ticket
    /// </summary>
    public Guid ResolvedBy { get; }

    /// <summary>
    /// When the ticket was resolved
    /// </summary>
    public DateTimeOffset ResolvedAt { get; }

    /// <summary>
    /// Resolution note explaining how the issue was fixed
    /// </summary>
    public string ResolutionNote { get; }

    public TicketResolvedEvent(
        Guid ticketId,
        Guid resolvedBy,
        DateTimeOffset resolvedAt,
        string resolutionNote)
        : base(ticketId)
    {
        TicketId = ticketId;
        ResolvedBy = resolvedBy;
        ResolvedAt = resolvedAt;
        ResolutionNote = resolutionNote;
    }
}
