using Flowertrack.Application.Common.Interfaces;
using Flowertrack.Domain.Common;
using Flowertrack.Domain.Entities;
using Flowertrack.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence;

/// <summary>
/// Application database context for FLOWerTRACK
/// </summary>
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<ServiceUser> ServiceUsers => Set<ServiceUser>();
    public DbSet<OrganizationUser> OrganizationUsers => Set<OrganizationUser>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore domain events - they should not be persisted
        modelBuilder.Ignore<DomainEvent>();

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
