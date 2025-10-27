using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Organization entity
/// </summary>
public sealed class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Organization?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(o => o.Name == name, ct);
    }

    public async Task<Organization?> GetByApiKeyAsync(string apiKey, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(o => o.ApiKey == apiKey, ct);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(o => o.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(o => o.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<bool> ApiKeyExistsAsync(string apiKey, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(o => o.ApiKey == apiKey);

        if (excludeId.HasValue)
        {
            query = query.Where(o => o.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<IReadOnlyList<Organization>> GetActiveOrganizationsAsync(CancellationToken ct = default)
    {
        return await DbSet
            .Where(o => o.ServiceStatus == ServiceStatus.Active)
            .OrderBy(o => o.Name)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Organization>> GetExpiredContractsAsync(CancellationToken ct = default)
    {
        return await DbSet
            .Where(o => o.ContractEndDate.HasValue && o.ContractEndDate.Value < DateTimeOffset.UtcNow)
            .OrderBy(o => o.ContractEndDate)
            .ToListAsync(ct);
    }
}
