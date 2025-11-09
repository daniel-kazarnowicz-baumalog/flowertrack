using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for TicketAttachment entity
/// </summary>
public sealed class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachment>
{
    public void Configure(EntityTypeBuilder<TicketAttachment> builder)
    {
        builder.ToTable("TicketAttachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedNever();

        // Foreign Keys
        builder.Property(a => a.TicketId)
            .IsRequired();

        builder.HasIndex(a => a.TicketId)
            .HasDatabaseName("IX_TicketAttachments_TicketId");

        builder.Property(a => a.UploadedBy)
            .IsRequired();

        builder.HasIndex(a => a.UploadedBy)
            .HasDatabaseName("IX_TicketAttachments_UploadedBy");

        // File properties
        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.StoragePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("application/octet-stream");

        builder.Property(a => a.FileSizeBytes)
            .IsRequired();

        // Audit fields from AuditableEntity
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.CreatedBy)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        builder.Property(a => a.UpdatedBy);

        builder.Property(a => a.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedAt);

        builder.Property(a => a.DeletedBy);

        // Indexes for common queries
        builder.HasIndex(a => new { a.TicketId, a.IsDeleted })
            .HasDatabaseName("IX_TicketAttachments_TicketId_IsDeleted");

        builder.HasIndex(a => a.StoragePath)
            .HasDatabaseName("IX_TicketAttachments_StoragePath");

        // Relationship to Ticket
        builder.HasOne(a => a.Ticket)
            .WithMany()
            .HasForeignKey(a => a.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore domain events collection
        builder.Ignore(a => a.DomainEvents);
    }
}
