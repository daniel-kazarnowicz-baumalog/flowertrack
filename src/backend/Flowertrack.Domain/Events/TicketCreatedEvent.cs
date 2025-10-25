using Flowertrack.Domain.Common;

namespace Flowertrack.Domain.Events;

using Flowertrack.Domain.Common;

/// <summary>
/// Event raised when a new ticket is created in the system.
/// This event is published immediately after ticket creation and contains all initial ticket data.
/// </summary>
public sealed class TicketCreatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the created ticket
    /// </summary>
    public Guid TicketId { get; }

    /// <summary>
    /// Human-readable ticket number (e.g., "TKT-2024-0001")
    /// </summary>
    public string TicketNumber { get; }

    /// <summary>
    /// Organization that owns this ticket
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Machine associated with this ticket
    /// </summary>
    public Guid MachineId { get; }

    /// <summary>
    /// User who created the ticket
    /// </summary>
    public Guid CreatedBy { get; }

    /// <summary>
    /// Priority level of the ticket
    /// </summary>
    public string Priority { get; }

    public TicketCreatedEvent(
        Guid ticketId,
        string ticketNumber,
        Guid organizationId,
        Guid machineId,
        Guid createdBy,
        string priority)
        : base(ticketId)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        Title = title;
        OrganizationId = organizationId;
        MachineId = machineId;
        CreatedBy = createdBy;
        Priority = priority;
    }
}
