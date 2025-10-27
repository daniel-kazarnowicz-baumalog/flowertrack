using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Application.Common.Interfaces;

/// <summary>
/// Interface for accessing application database context.
/// Provides read-only access to DbSets for query operations.
/// </summary>
public interface IApplicationDbContext
{
    /// <summary>
    /// Gets the Organizations DbSet.
    /// </summary>
    DbSet<Organization> Organizations { get; }

    /// <summary>
    /// Gets the Machines DbSet.
    /// </summary>
    DbSet<Machine> Machines { get; }

    /// <summary>
    /// Gets the ServiceUsers DbSet.
    /// </summary>
    DbSet<ServiceUser> ServiceUsers { get; }

    /// <summary>
    /// Gets the OrganizationUsers DbSet.
    /// </summary>
    DbSet<OrganizationUser> OrganizationUsers { get; }

    /// <summary>
    /// Gets the Tickets DbSet.
    /// </summary>
    DbSet<Ticket> Tickets { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
