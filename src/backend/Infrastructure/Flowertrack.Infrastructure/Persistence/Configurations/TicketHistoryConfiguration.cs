using Flowertrack.Domain.Entities.Tickets;
using Flowertrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for TicketHistory
/// </summary>
public sealed class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(EntityTypeBuilder<TicketHistory> builder)
    {
        builder.ToTable("ticket_history");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(h => h.TicketId)
            .HasColumnName("ticket_id")
            .IsRequired();

        builder.Property(h => h.HistoryType)
            .HasColumnName("history_type")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(h => h.UserId)
            .HasColumnName("user_id");

        builder.Property(h => h.UserName)
            .HasColumnName("user_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(h => h.UserType)
            .HasColumnName("user_type")
            .HasMaxLength(50);

        builder.Property(h => h.OldValue)
            .HasColumnName("old_value")
            .HasMaxLength(500);

        builder.Property(h => h.NewValue)
            .HasColumnName("new_value")
            .HasMaxLength(500);

        builder.Property(h => h.Content)
            .HasColumnName("content")
            .HasMaxLength(4000);

        builder.Property(h => h.AttachmentFileName)
            .HasColumnName("attachment_file_name")
            .HasMaxLength(500);

        builder.Property(h => h.AttachmentFilePath)
            .HasColumnName("attachment_file_path")
            .HasMaxLength(1000);

        builder.Property(h => h.AttachmentFileSize)
            .HasColumnName("attachment_file_size");

        builder.Property(h => h.IsInternal)
            .HasColumnName("is_internal")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(h => h.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(h => h.TicketId)
            .HasDatabaseName("ix_ticket_history_ticket_id");

        builder.HasIndex(h => h.UserId)
            .HasDatabaseName("ix_ticket_history_user_id");

        builder.HasIndex(h => h.CreatedAt)
            .HasDatabaseName("ix_ticket_history_created_at");

        builder.HasIndex(h => new { h.TicketId, h.CreatedAt })
            .HasDatabaseName("ix_ticket_history_ticket_created");

        // Foreign key to Ticket
        builder.HasOne(h => h.Ticket)
            .WithMany()
            .HasForeignKey(h => h.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
