namespace Flowertrack.Api.Domain.Events;

using Flowertrack.Api.Domain.Common;

/// <summary>
/// Event raised when a new ticket is created in the system.
/// This event is published immediately after ticket creation and contains all initial ticket data.
/// </summary>
public sealed record TicketCreatedEvent : DomainEvent
{
    /// <summary>
    /// Unique identifier of the created ticket
    /// </summary>
    public Guid TicketId { get; init; }

    /// <summary>
    /// Human-readable ticket number (e.g., "TKT-2024-0001")
    /// </summary>
    public required string TicketNumber { get; init; }

    /// <summary>
    /// Organization that owns this ticket
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Machine associated with this ticket
    /// </summary>
    public Guid MachineId { get; init; }

    /// <summary>
    /// User who created the ticket
    /// </summary>
    public Guid CreatedBy { get; init; }

    /// <summary>
    /// Priority level of the ticket
    /// </summary>
    public required string Priority { get; init; }

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
        OrganizationId = organizationId;
        MachineId = machineId;
        CreatedBy = createdBy;
        Priority = priority;
    }
}
