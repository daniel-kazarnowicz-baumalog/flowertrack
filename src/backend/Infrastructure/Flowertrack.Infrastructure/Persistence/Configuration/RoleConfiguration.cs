using Flowertrack.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configuration;

/// <summary>
/// Entity Framework configuration for Role entity
/// </summary>
public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(r => r.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("ix_roles_name");

        // Seed data
        builder.HasData(
            new { Id = 1, Name = "ServiceAdministrator", Description = "Service team administrator with full access to service management" },
            new { Id = 2, Name = "ServiceTechnician", Description = "Service technician with ticket management and resolution capabilities" },
            new { Id = 3, Name = "OrganizationAdministrator", Description = "Organization administrator with team and machine management access" },
            new { Id = 4, Name = "Operator", Description = "Machine operator with basic ticket creation and viewing capabilities" }
        );
    }
}
