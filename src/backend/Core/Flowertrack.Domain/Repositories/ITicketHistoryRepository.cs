using Flowertrack.Domain.Entities.Tickets;

namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for TicketHistory entity
/// </summary>
public interface ITicketHistoryRepository : IRepository<TicketHistory>
{
    /// <summary>
    /// Gets all history entries for a specific ticket, ordered by creation date
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="includeInternal">Whether to include internal notes (for service users)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of history entries</returns>
    Task<IReadOnlyList<TicketHistory>> GetByTicketIdAsync(
        Guid ticketId, 
        bool includeInternal = false, 
        CancellationToken ct = default);

    /// <summary>
    /// Gets paginated history entries for a specific ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="includeInternal">Whether to include internal notes</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Paginated list of history entries</returns>
    Task<(IReadOnlyList<TicketHistory> Items, int TotalCount)> GetPagedByTicketIdAsync(
        Guid ticketId,
        int pageNumber,
        int pageSize,
        bool includeInternal = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets comments and notes for a specific ticket
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="includeInternal">Whether to include internal notes</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of comments and notes</returns>
    Task<IReadOnlyList<TicketHistory>> GetCommentsAndNotesAsync(
        Guid ticketId,
        bool includeInternal = false,
        CancellationToken ct = default);

    /// <summary>
    /// Gets all history entries created by a specific user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of history entries</returns>
    Task<IReadOnlyList<TicketHistory>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct = default);

    /// <summary>
    /// Adds multiple history entries in a single operation
    /// </summary>
    /// <param name="entries">History entries to add</param>
    /// <param name="ct">Cancellation token</param>
    Task AddRangeAsync(IEnumerable<TicketHistory> entries, CancellationToken ct = default);
}
