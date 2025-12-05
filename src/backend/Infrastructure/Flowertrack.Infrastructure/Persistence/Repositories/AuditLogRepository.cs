using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for AuditLog entity.
/// Note: AuditLog does not support Update or Delete operations by design.
/// </summary>
public sealed class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<AuditLog> _dbSet;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<AuditLog>();
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(auditLog, ct);
        return auditLog;
    }

    public async Task AddRangeAsync(IEnumerable<AuditLog> auditLogs, CancellationToken ct = default)
    {
        await _dbSet.AddRangeAsync(auditLogs, ct);
    }

    public async Task<AuditLog?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByUserIdAsync(
        Guid userId,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByResourceAsync(
        string resourceType,
        string resourceId,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(a => a.ResourceType == resourceType && a.ResourceId == resourceId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByActionTypeAsync(
        string actionType,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(a => a.ActionType == actionType)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByDateRangeAsync(
        DateTimeOffset from,
        DateTimeOffset to,
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        return await _dbSet
            .Where(a => a.CreatedAt >= from && a.CreatedAt <= to)
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AuditLog>> SearchAsync(
        Guid? userId = null,
        string? actionType = null,
        string? resourceType = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        bool? isSuccess = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(actionType))
        {
            query = query.Where(a => a.ActionType == actionType);
        }

        if (!string.IsNullOrWhiteSpace(resourceType))
        {
            query = query.Where(a => a.ResourceType == resourceType);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= to.Value);
        }

        if (isSuccess.HasValue)
        {
            query = query.Where(a => a.IsSuccess == isSuccess.Value);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public async Task<int> GetSearchCountAsync(
        Guid? userId = null,
        string? actionType = null,
        string? resourceType = null,
        DateTimeOffset? from = null,
        DateTimeOffset? to = null,
        bool? isSuccess = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(actionType))
        {
            query = query.Where(a => a.ActionType == actionType);
        }

        if (!string.IsNullOrWhiteSpace(resourceType))
        {
            query = query.Where(a => a.ResourceType == resourceType);
        }

        if (from.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= to.Value);
        }

        if (isSuccess.HasValue)
        {
            query = query.Where(a => a.IsSuccess == isSuccess.Value);
        }

        return await query.CountAsync(ct);
    }

    public async Task<int> GetFailedLoginCountAsync(
        string userEmail,
        DateTimeOffset since,
        CancellationToken ct = default)
    {
        return await _dbSet
            .CountAsync(a =>
                a.UserEmail == userEmail
                && a.ActionType == AuditActionTypes.LoginFailed
                && a.CreatedAt >= since, ct);
    }
}
