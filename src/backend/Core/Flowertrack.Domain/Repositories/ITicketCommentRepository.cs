namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for TicketComment entity
/// </summary>
public interface ITicketCommentRepository : IRepository<Entities.TicketComment>
{
    /// <summary>
    /// Gets comments for a specific ticket with pagination
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="includeInternal">Whether to include internal comments (for service users only)</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of comments ordered by CreatedAt DESC</returns>
    Task<(IReadOnlyList<Entities.TicketComment> Comments, int TotalCount)> GetByTicketIdAsync(
        Guid ticketId,
        bool includeInternal,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
