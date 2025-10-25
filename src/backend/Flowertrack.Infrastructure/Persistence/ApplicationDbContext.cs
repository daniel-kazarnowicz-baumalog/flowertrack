using Flowertrack.Domain.Common;
using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flowertrack.Infrastructure.Persistence;

/// <summary>
/// Application database context for FLOWerTRACK
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Organization> Organizations => Set<Organization>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignore domain events - they should not be persisted
        modelBuilder.Ignore<DomainEvent>();

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
