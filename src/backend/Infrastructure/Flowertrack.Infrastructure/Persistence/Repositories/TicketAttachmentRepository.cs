using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TicketAttachment entity
/// </summary>
public sealed class TicketAttachmentRepository : Repository<TicketAttachment>, ITicketAttachmentRepository
{
    public TicketAttachmentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<TicketAttachment> Attachments, int TotalCount)> GetByTicketIdAsync(
        Guid ticketId,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(a => a.TicketId == ticketId && !a.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var attachments = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (attachments, totalCount);
    }
}
