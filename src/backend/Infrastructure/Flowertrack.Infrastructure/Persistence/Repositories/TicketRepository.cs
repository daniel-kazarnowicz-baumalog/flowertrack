using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using Flowertrack.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Ticket entity
/// </summary>
public sealed class TicketRepository : Repository<Ticket>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Ticket?> GetByTicketNumberAsync(TicketNumber ticketNumber, CancellationToken ct = default)
    {
        return await DbSet
        .FirstOrDefaultAsync(t => t.TicketNumber == ticketNumber, ct);
    }

    public async Task<IReadOnlyList<Ticket>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default)
    {
        return await DbSet
   .Where(t => t.OrganizationId == organizationId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Ticket>> GetByMachineIdAsync(Guid machineId, CancellationToken ct = default)
    {
        return await DbSet
    .Where(t => t.MachineId == machineId)
         .OrderByDescending(t => t.CreatedAt)
        .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Ticket>> GetByAssignedUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await DbSet
            .Where(t => t.AssignedToUserId == userId)
 .OrderByDescending(t => t.CreatedAt)
.ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Ticket>> GetByStatusAsync(TicketStatus status, CancellationToken ct = default)
    {
        return await DbSet
       .Where(t => t.Status == status)
          .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> GetNextSequentialNumberAsync(int year, CancellationToken ct = default)
    {
        var maxNumber = await DbSet
      .Where(t => t.TicketNumber.Year == year)
        .MaxAsync(t => (int?)t.TicketNumber.Sequential, ct);

    return (maxNumber ?? 0) + 1;
    }

    public async Task<bool> TicketNumberExistsAsync(TicketNumber ticketNumber, CancellationToken ct = default)
    {
   return await DbSet
   .AnyAsync(t => t.TicketNumber == ticketNumber, ct);
    }

    public async Task<bool> HasActiveTicketsForOrganizationAsync(Guid organizationId, CancellationToken ct = default)
    {
        return await DbSet
            .AnyAsync(t => t.OrganizationId == organizationId && t.Status != TicketStatus.Closed, ct);
    }
}
