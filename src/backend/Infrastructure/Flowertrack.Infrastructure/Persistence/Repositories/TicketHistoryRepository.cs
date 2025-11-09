using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using Flowertrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for TicketHistory entity
/// </summary>
public sealed class TicketHistoryRepository(ApplicationDbContext context) 
    : Repository<TicketHistory>(context), ITicketHistoryRepository
{
    public async Task<IReadOnlyList<TicketHistory>> GetByTicketIdAsync(
        Guid ticketId,
        bool includeInternal = false,
        CancellationToken ct = default)
    {
        var query = Context.Set<TicketHistory>()
            .Where(h => h.TicketId == ticketId);

        if (!includeInternal)
        {
            query = query.Where(h => !h.IsInternal);
        }

        return await query
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<(IReadOnlyList<TicketHistory> Items, int TotalCount)> GetPagedByTicketIdAsync(
        Guid ticketId,
        int pageNumber,
        int pageSize,
        bool includeInternal = false,
        CancellationToken ct = default)
    {
        var query = Context.Set<TicketHistory>()
            .Where(h => h.TicketId == ticketId);

        if (!includeInternal)
        {
            query = query.Where(h => !h.IsInternal);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(h => h.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<TicketHistory>> GetCommentsAndNotesAsync(
        Guid ticketId,
        bool includeInternal = false,
        CancellationToken ct = default)
    {
        var query = Context.Set<TicketHistory>()
            .Where(h => h.TicketId == ticketId &&
                       (h.HistoryType == TicketHistoryType.Comment || 
                        h.HistoryType == TicketHistoryType.Note));

        if (!includeInternal)
        {
            query = query.Where(h => !h.IsInternal);
        }

        return await query
            .OrderBy(h => h.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TicketHistory>> GetByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        return await Context.Set<TicketHistory>()
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<TicketHistory> entries, CancellationToken ct = default)
    {
        await Context.Set<TicketHistory>().AddRangeAsync(entries, ct);
    }
}
