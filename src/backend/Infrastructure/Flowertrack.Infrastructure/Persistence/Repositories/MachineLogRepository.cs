using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for MachineLog entity
/// </summary>
public sealed class MachineLogRepository : Repository<MachineLog>, IMachineLogRepository
{
    public MachineLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<MachineLog>> GetByMachineIdAsync(
        Guid machineId,
        CancellationToken ct = default)
    {
        return await DbSet
            .Where(ml => ml.MachineId == machineId && !ml.IsDeleted)
            .OrderByDescending(ml => ml.ReceivedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MachineLog>> GetByMachineIdAndDateRangeAsync(
        Guid machineId,
        DateTimeOffset from,
        DateTimeOffset to,
        CancellationToken ct = default)
    {
        return await DbSet
            .Where(ml => ml.MachineId == machineId
                && ml.ReceivedAt >= from
                && ml.ReceivedAt <= to
                && !ml.IsDeleted)
            .OrderByDescending(ml => ml.ReceivedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MachineLog>> GetAlarmsByMachineIdAsync(
        Guid machineId,
        CancellationToken ct = default)
    {
        return await DbSet
            .Where(ml => ml.MachineId == machineId
                && ml.LogType == "ALARM"
                && !ml.IsDeleted)
            .OrderByDescending(ml => ml.ReceivedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<MachineLog>> GetUnprocessedAsync(
        int limit = 100,
        CancellationToken ct = default)
    {
        return await DbSet
            .Where(ml => !ml.IsProcessed && !ml.IsDeleted)
            .OrderBy(ml => ml.ReceivedAt)
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task<MachineLog?> GetLatestByMachineIdAsync(
        Guid machineId,
        CancellationToken ct = default)
    {
        return await DbSet
            .Where(ml => ml.MachineId == machineId && !ml.IsDeleted)
            .OrderByDescending(ml => ml.ReceivedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<MachineLog>> GetByMachineIdPagedAsync(
        Guid machineId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        return await DbSet
            .Where(ml => ml.MachineId == machineId && !ml.IsDeleted)
            .OrderByDescending(ml => ml.ReceivedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> GetCountByMachineIdAsync(
        Guid machineId,
        CancellationToken ct = default)
    {
        return await DbSet
            .CountAsync(ml => ml.MachineId == machineId && !ml.IsDeleted, ct);
    }
}
