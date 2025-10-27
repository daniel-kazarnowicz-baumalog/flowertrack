using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using MachineStatusEnum = Flowertrack.Domain.Enums.MachineStatus;
using MachineStatusVO = Flowertrack.Domain.ValueObjects.MachineStatus;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Machine entity
/// </summary>
public sealed class MachineRepository : Repository<Machine>, IMachineRepository
{
    public MachineRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Machine?> GetBySerialNumberAsync(string serialNumber, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(m => m.SerialNumber == serialNumber, ct);
    }

    public async Task<Machine?> GetByApiTokenAsync(string apiToken, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(m => m.ApiToken != null && m.ApiToken.Value == apiToken, ct);
    }

    public async Task<IReadOnlyList<Machine>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default)
    {
        return await DbSet
            .Where(m => m.OrganizationId == organizationId)
            .OrderBy(m => m.SerialNumber)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Machine>> GetByStatusAsync(MachineStatusEnum status, CancellationToken ct = default)
    {
        // Convert enum to ValueObject version for comparison
        var statusVO = (MachineStatusVO)status;
        return await DbSet
            .Where(m => m.Status == statusVO)
            .OrderBy(m => m.SerialNumber)
            .ToListAsync(ct);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(m => m.SerialNumber == serialNumber);

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<bool> ApiTokenExistsAsync(string apiToken, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(m => m.ApiToken != null && m.ApiToken.Value == apiToken);

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<IReadOnlyList<Machine>> GetMachinesDueForMaintenanceAsync(DateOnly beforeDate, CancellationToken ct = default)
    {
        return await DbSet
            .Where(m => m.NextMaintenanceDate.HasValue && m.NextMaintenanceDate.Value <= beforeDate)
            .OrderBy(m => m.NextMaintenanceDate)
            .ToListAsync(ct);
    }
}
