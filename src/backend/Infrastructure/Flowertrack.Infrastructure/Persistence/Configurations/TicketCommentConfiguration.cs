using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for TicketComment entity
/// </summary>
public sealed class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("TicketComments");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        // Foreign Keys
        builder.Property(c => c.TicketId)
            .IsRequired();

        builder.HasIndex(c => c.TicketId)
            .HasDatabaseName("IX_TicketComments_TicketId");

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.HasIndex(c => c.UserId)
            .HasDatabaseName("IX_TicketComments_UserId");

        // Content
        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(5000);

        // IsInternal flag
        builder.Property(c => c.IsInternal)
            .IsRequired()
            .HasDefaultValue(false);

        // Audit fields from AuditableEntity
        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        builder.Property(c => c.UpdatedBy);

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.DeletedAt);

        builder.Property(c => c.DeletedBy);

        // Indexes for common queries
        builder.HasIndex(c => new { c.TicketId, c.IsDeleted })
            .HasDatabaseName("IX_TicketComments_TicketId_IsDeleted");

        builder.HasIndex(c => new { c.TicketId, c.IsInternal, c.IsDeleted })
            .HasDatabaseName("IX_TicketComments_TicketId_IsInternal_IsDeleted");

        // Relationship to Ticket
        builder.HasOne(c => c.Ticket)
            .WithMany()
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore domain events collection
        builder.Ignore(c => c.DomainEvents);
    }
}
