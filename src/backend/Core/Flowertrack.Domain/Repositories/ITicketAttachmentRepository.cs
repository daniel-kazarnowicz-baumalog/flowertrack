namespace Flowertrack.Domain.Repositories;

/// <summary>
/// Repository interface for TicketAttachment entity
/// </summary>
public interface ITicketAttachmentRepository : IRepository<Entities.TicketAttachment>
{
    /// <summary>
    /// Gets attachments for a specific ticket with pagination
    /// </summary>
    /// <param name="ticketId">Ticket ID</param>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of attachments ordered by CreatedAt DESC</returns>
    Task<(IReadOnlyList<Entities.TicketAttachment> Attachments, int TotalCount)> GetByTicketIdAsync(
        Guid ticketId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
}
