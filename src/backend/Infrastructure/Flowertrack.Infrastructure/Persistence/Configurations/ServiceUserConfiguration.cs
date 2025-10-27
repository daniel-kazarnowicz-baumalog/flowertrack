using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for ServiceUser entity
/// </summary>
public sealed class ServiceUserConfiguration : IEntityTypeConfiguration<ServiceUser>
{
    public void Configure(EntityTypeBuilder<ServiceUser> builder)
    {
        builder.ToTable("ServiceUsers");

        builder.HasKey(su => su.Id);

        builder.Property(su => su.Id)
            .ValueGeneratedNever(); // ID comes from Supabase Auth

        // Basic properties
        builder.Property(su => su.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(su => su.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Email - Value Object
        builder.OwnsOne(su => su.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_ServiceUsers_Email");
        });

        builder.Property(su => su.PhoneNumber)
            .HasMaxLength(50);

        // Status - stored as string
        builder.Property(su => su.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(su => su.Specialization)
            .HasMaxLength(100);

        builder.Property(su => su.IsAvailable)
            .IsRequired()
            .HasDefaultValue(false);

        // Auditable fields
        builder.Property(su => su.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(su => su.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(su => su.CreatedBy);
        builder.Property(su => su.UpdatedBy);

        // Ignore domain events
        builder.Ignore(su => su.DomainEvents);
    }
}
