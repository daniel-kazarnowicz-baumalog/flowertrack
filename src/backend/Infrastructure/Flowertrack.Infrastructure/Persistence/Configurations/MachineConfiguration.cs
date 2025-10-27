using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Machine entity
/// </summary>
public sealed class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.ToTable("Machines");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        // Serial Number - unique constraint
        builder.Property(m => m.SerialNumber)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(m => m.SerialNumber)
            .IsUnique()
            .HasDatabaseName("IX_Machines_SerialNumber");

        // Basic properties
        builder.Property(m => m.Brand)
            .HasMaxLength(100);

        builder.Property(m => m.Model)
            .HasMaxLength(100);

        builder.Property(m => m.Location)
            .HasMaxLength(255);

        // Status - stored as string
        builder.Property(m => m.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // API Token - Value Object
        builder.OwnsOne(m => m.ApiToken, apiToken =>
        {
            apiToken.Property(a => a.Value)
                .HasColumnName("ApiToken")
                .HasMaxLength(255);

            apiToken.HasIndex(a => a.Value)
                .IsUnique()
                .HasDatabaseName("IX_Machines_ApiToken");
        });

        // Maintenance dates
        builder.Property(m => m.LastMaintenanceDate)
            .HasColumnType("date");

        builder.Property(m => m.NextMaintenanceDate)
            .HasColumnType("date");

        // Foreign Keys
        builder.Property(m => m.OrganizationId)
            .IsRequired();

        builder.HasIndex(m => m.OrganizationId)
            .HasDatabaseName("IX_Machines_OrganizationId");

        builder.Property(m => m.MaintenanceIntervalId)
            .IsRequired(false);

        // Auditable fields
        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(m => m.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(m => m.CreatedBy);
        builder.Property(m => m.UpdatedBy);

        // Ignore domain events collection (not persisted)
        builder.Ignore(m => m.DomainEvents);
    }
}
