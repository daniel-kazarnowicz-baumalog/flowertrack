using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for MachineLog entity
/// </summary>
public sealed class MachineLogConfiguration : IEntityTypeConfiguration<MachineLog>
{
    public void Configure(EntityTypeBuilder<MachineLog> builder)
    {
        builder.ToTable("machine_logs");

        builder.HasKey(ml => ml.Id);

        builder.Property(ml => ml.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        // Foreign key to Machine
        builder.Property(ml => ml.MachineId)
            .HasColumnName("machine_id")
            .IsRequired();

        builder.HasIndex(ml => ml.MachineId)
            .HasDatabaseName("IX_machine_logs_machine_id");

        // Timestamp fields
        builder.Property(ml => ml.ReceivedAt)
            .HasColumnName("received_at")
            .IsRequired();

        builder.HasIndex(ml => ml.ReceivedAt)
            .HasDatabaseName("IX_machine_logs_received_at");

        builder.Property(ml => ml.MachineTimestamp)
            .HasColumnName("machine_timestamp");

        // Log content as JSON
        builder.Property(ml => ml.LogContent)
            .HasColumnName("log_content")
            .HasColumnType("jsonb")
            .IsRequired()
            .HasMaxLength(65535);

        // Log type
        builder.Property(ml => ml.LogType)
            .HasColumnName("log_type")
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(ml => ml.LogType)
            .HasDatabaseName("IX_machine_logs_log_type");

        // Status
        builder.Property(ml => ml.Status)
            .HasColumnName("status")
            .HasMaxLength(50);

        builder.HasIndex(ml => ml.Status)
            .HasDatabaseName("IX_machine_logs_status");

        // Alarm fields
        builder.Property(ml => ml.AlarmCode)
            .HasColumnName("alarm_code")
            .HasMaxLength(100);

        builder.Property(ml => ml.AlarmMessage)
            .HasColumnName("alarm_message")
            .HasMaxLength(500);

        builder.HasIndex(ml => ml.AlarmCode)
            .HasDatabaseName("IX_machine_logs_alarm_code");

        // Severity
        builder.Property(ml => ml.Severity)
            .HasColumnName("severity")
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(ml => ml.Severity)
            .HasDatabaseName("IX_machine_logs_severity");

        // Processing status
        builder.Property(ml => ml.IsProcessed)
            .HasColumnName("is_processed")
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(ml => ml.IsProcessed)
            .HasDatabaseName("IX_machine_logs_is_processed")
            .HasFilter("is_processed = false");

        // Auditable fields
        builder.Property(ml => ml.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(ml => ml.CreatedBy)
            .HasColumnName("created_by");

        builder.Property(ml => ml.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(ml => ml.UpdatedBy)
            .HasColumnName("updated_by");

        builder.Property(ml => ml.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(ml => ml.DeletedAt)
            .HasColumnName("deleted_at");

        builder.Property(ml => ml.DeletedBy)
            .HasColumnName("deleted_by");

        // Navigation property
        builder.HasOne(ml => ml.Machine)
            .WithMany()
            .HasForeignKey(ml => ml.MachineId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events collection (not persisted)
        builder.Ignore(ml => ml.DomainEvents);

        // Composite index for common queries
        builder.HasIndex(ml => new { ml.MachineId, ml.ReceivedAt })
            .HasDatabaseName("IX_machine_logs_machine_received")
            .IsDescending(false, true);
    }
}
