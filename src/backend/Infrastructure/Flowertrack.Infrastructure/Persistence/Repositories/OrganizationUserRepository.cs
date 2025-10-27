using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for OrganizationUser entity
/// </summary>
public sealed class OrganizationUserRepository : Repository<OrganizationUser>, IOrganizationUserRepository
{
    public OrganizationUserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<OrganizationUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email.Value == email, ct);
    }

    public async Task<IReadOnlyList<OrganizationUser>> GetByOrganizationIdAsync(Guid organizationId, CancellationToken ct = default)
    {
        return await DbSet
            .Where(u => u.OrganizationId == organizationId)
            .OrderBy(u => u.Email.Value)
            .ToListAsync(ct);
    }

    public async Task<bool> EmailExistsInOrganizationAsync(string email, Guid organizationId, CancellationToken ct = default)
    {
        return await DbSet
            .AnyAsync(u => u.Email.Value == email && u.OrganizationId == organizationId, ct);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = DbSet.Where(u => u.Email.Value == email);

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }
}
