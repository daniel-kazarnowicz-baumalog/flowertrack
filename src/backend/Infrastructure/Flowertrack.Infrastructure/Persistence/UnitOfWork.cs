using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Flowertrack.Infrastructure.Persistence;

/// <summary>
/// Implementation of Unit of Work pattern
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(
        ApplicationDbContext dbContext,
        IOrganizationRepository organizationRepository,
        IMachineRepository machineRepository,
        IServiceUserRepository serviceUserRepository,
        IOrganizationUserRepository organizationUserRepository,
        ITicketRepository ticketRepository)
    {
        _dbContext = dbContext;
        Organizations = organizationRepository;
        Machines = machineRepository;
        ServiceUsers = serviceUserRepository;
        OrganizationUsers = organizationUserRepository;
        Tickets = ticketRepository;
    }

    public IOrganizationRepository Organizations { get; }
    public IMachineRepository Machines { get; }
    public IServiceUserRepository ServiceUsers { get; }
    public IOrganizationUserRepository OrganizationUsers { get; }
    public ITicketRepository Tickets { get; }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        return await _dbContext.SaveChangesAsync(ct);
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync(ct);
    }

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(ct);

            if (_transaction != null)
            {
                await _transaction.CommitAsync(ct);
            }
        }
        catch
        {
            await RollbackTransactionAsync(ct);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(ct);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _dbContext.Dispose();
    }
}
