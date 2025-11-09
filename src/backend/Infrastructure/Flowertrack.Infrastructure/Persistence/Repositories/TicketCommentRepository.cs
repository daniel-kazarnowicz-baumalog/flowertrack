using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TicketComment entity
/// </summary>
public sealed class TicketCommentRepository : Repository<TicketComment>, ITicketCommentRepository
{
    public TicketCommentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<(IReadOnlyList<TicketComment> Comments, int TotalCount)> GetByTicketIdAsync(
        Guid ticketId,
        bool includeInternal,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Where(c => c.TicketId == ticketId && !c.IsDeleted);

        // Filter out internal comments if not allowed
        if (!includeInternal)
        {
            query = query.Where(c => !c.IsInternal);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (comments, totalCount);
    }
}
