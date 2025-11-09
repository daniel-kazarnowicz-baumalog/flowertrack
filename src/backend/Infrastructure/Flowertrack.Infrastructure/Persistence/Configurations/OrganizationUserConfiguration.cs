using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for OrganizationUser entity
/// </summary>
public sealed class OrganizationUserConfiguration : IEntityTypeConfiguration<OrganizationUser>
{
    public void Configure(EntityTypeBuilder<OrganizationUser> builder)
    {
        builder.ToTable("OrganizationUsers");

        builder.HasKey(ou => ou.Id);

        builder.Property(ou => ou.Id)
            .ValueGeneratedOnAdd(); // Internal domain ID

        // Supabase User ID - nullable FK to auth.users
        builder.Property(ou => ou.SupabaseUserId)
            .IsRequired(false);

        builder.HasIndex(ou => ou.SupabaseUserId)
            .IsUnique()
            .HasFilter("\"SupabaseUserId\" IS NOT NULL")
            .HasDatabaseName("IX_OrganizationUsers_SupabaseUserId");

        // Activation fields
        builder.Property(ou => ou.IsActivated)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ou => ou.InvitationToken)
            .HasMaxLength(500);

        builder.Property(ou => ou.InvitationTokenExpiresAt);

        // Basic properties
        builder.Property(ou => ou.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ou => ou.LastName)
            .IsRequired()
            .HasMaxLength(100);

        // Email - Value Object
        builder.OwnsOne(ou => ou.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("Email")
                .IsRequired()
                .HasMaxLength(255);

            email.HasIndex(e => e.Value)
                .IsUnique()
                .HasDatabaseName("IX_OrganizationUsers_Email");
        });

        builder.Property(ou => ou.PhoneNumber)
            .HasMaxLength(50);

        // Organization relationship
        builder.Property(ou => ou.OrganizationId)
            .IsRequired();

        builder.HasIndex(ou => ou.OrganizationId)
            .HasDatabaseName("IX_OrganizationUsers_OrganizationId");

        // Role
        builder.Property(ou => ou.Role)
            .IsRequired()
            .HasMaxLength(50);

        // Status - stored as string
        builder.Property(ou => ou.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Auditable fields
        builder.Property(ou => ou.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(ou => ou.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(ou => ou.CreatedBy);
        builder.Property(ou => ou.UpdatedBy);

        // Ignore domain events
        builder.Ignore(ou => ou.DomainEvents);
    }
}
