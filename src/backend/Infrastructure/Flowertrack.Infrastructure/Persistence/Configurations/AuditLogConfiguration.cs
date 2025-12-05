using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for AuditLog entity
/// </summary>
public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(a => a.Id);

        // Auto-increment Id for high-volume writes
        builder.Property(a => a.Id)
            .HasColumnName("id")
            .UseIdentityAlwaysColumn();

        // User information
        builder.Property(a => a.UserId)
            .HasColumnName("user_id");

        builder.HasIndex(a => a.UserId)
            .HasDatabaseName("IX_audit_logs_user_id");

        builder.Property(a => a.UserEmail)
            .HasColumnName("user_email")
            .HasMaxLength(255);

        // Action information
        builder.Property(a => a.ActionType)
            .HasColumnName("action_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(a => a.ActionType)
            .HasDatabaseName("IX_audit_logs_action_type");

        // Resource information
        builder.Property(a => a.ResourceType)
            .HasColumnName("resource_type")
            .HasMaxLength(100);

        builder.Property(a => a.ResourceId)
            .HasColumnName("resource_id")
            .HasMaxLength(100);

        builder.HasIndex(a => new { a.ResourceType, a.ResourceId })
            .HasDatabaseName("IX_audit_logs_resource");

        // Description
        builder.Property(a => a.Description)
            .HasColumnName("description")
            .HasMaxLength(1000);

        // Old and new values as JSON
        builder.Property(a => a.OldValue)
            .HasColumnName("old_value")
            .HasColumnType("jsonb");

        builder.Property(a => a.NewValue)
            .HasColumnName("new_value")
            .HasColumnType("jsonb");

        // HTTP context
        builder.Property(a => a.IpAddress)
            .HasColumnName("ip_address")
            .HasMaxLength(45); // IPv6 max length

        builder.Property(a => a.UserAgent)
            .HasColumnName("user_agent")
            .HasMaxLength(500);

        builder.Property(a => a.HttpMethod)
            .HasColumnName("http_method")
            .HasMaxLength(10);

        builder.Property(a => a.RequestPath)
            .HasColumnName("request_path")
            .HasMaxLength(2048);

        builder.Property(a => a.CorrelationId)
            .HasColumnName("correlation_id")
            .HasMaxLength(50);

        builder.HasIndex(a => a.CorrelationId)
            .HasDatabaseName("IX_audit_logs_correlation_id");

        // Timestamp
        builder.Property(a => a.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(a => a.CreatedAt)
            .HasDatabaseName("IX_audit_logs_created_at");

        // Success status
        builder.Property(a => a.IsSuccess)
            .HasColumnName("is_success")
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(a => a.IsSuccess)
            .HasDatabaseName("IX_audit_logs_is_success")
            .HasFilter("is_success = false");

        builder.Property(a => a.ErrorMessage)
            .HasColumnName("error_message")
            .HasMaxLength(2000);

        // Composite indexes for common queries
        builder.HasIndex(a => new { a.UserId, a.CreatedAt })
            .HasDatabaseName("IX_audit_logs_user_created")
            .IsDescending(false, true);

        builder.HasIndex(a => new { a.ActionType, a.CreatedAt })
            .HasDatabaseName("IX_audit_logs_action_created")
            .IsDescending(false, true);
    }
}
