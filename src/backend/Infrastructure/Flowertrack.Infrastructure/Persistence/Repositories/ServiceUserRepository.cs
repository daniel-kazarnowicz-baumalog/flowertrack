using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Enums;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for ServiceUser entity
/// </summary>
public sealed class ServiceUserRepository : Repository<ServiceUser>, IServiceUserRepository
{
    public ServiceUserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ServiceUser?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email.Value == email, ct);
    }

    public async Task<IReadOnlyList<ServiceUser>> GetActiveUsersAsync(CancellationToken ct = default)
    {
        return await DbSet
            .Where(u => u.Status == UserStatus.Active)
            .OrderBy(u => u.Email.Value)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ServiceUser>> GetAvailableUsersAsync(CancellationToken ct = default)
    {
        return await DbSet
            .Where(u => u.Status == UserStatus.Active)
            .OrderBy(u => u.Email.Value)
            .ToListAsync(ct);
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
