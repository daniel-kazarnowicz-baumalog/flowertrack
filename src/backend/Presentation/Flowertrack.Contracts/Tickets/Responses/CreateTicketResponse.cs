namespace Flowertrack.Contracts.Tickets.Responses;

/// <summary>
/// Response after successfully creating a ticket
/// </summary>
public sealed record CreateTicketResponse
{
    /// <summary>
    /// Unique identifier of the created ticket
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Unique ticket number (e.g., TICK-2025-00001)
    /// </summary>
    public string TicketNumber { get; init; } = string.Empty;

    /// <summary>
    /// Ticket title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Current status of the ticket
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Timestamp when the ticket was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }
}
