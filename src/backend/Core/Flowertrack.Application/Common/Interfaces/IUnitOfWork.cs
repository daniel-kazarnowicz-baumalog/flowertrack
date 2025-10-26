using Flowertrack.Domain.Repositories;

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface.
/// Coordinates the work of multiple repositories and maintains transaction consistency.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Gets the Tickets repository.
    /// </summary>
    ITicketRepository Tickets { get; }

    /// <summary>
    /// Gets the Organizations repository.
    /// </summary>
    IOrganizationRepository Organizations { get; }

    /// <summary>
    /// Gets the Machines repository.
    /// </summary>
    IMachineRepository Machines { get; }

    /// <summary>
    /// Gets the ServiceUsers repository.
    /// </summary>
    IServiceUserRepository ServiceUsers { get; }

    /// <summary>
    /// Gets the OrganizationUsers repository.
    /// </summary>
    IOrganizationUserRepository OrganizationUsers { get; }

    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task BeginTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task CommitTransactionAsync(CancellationToken ct = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
