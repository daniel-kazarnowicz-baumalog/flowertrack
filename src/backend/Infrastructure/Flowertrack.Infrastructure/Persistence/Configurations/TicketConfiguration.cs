using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for Ticket entity
/// </summary>
public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
   .ValueGeneratedNever();

        // Title
        builder.Property(t => t.Title)
            .IsRequired()
 .HasMaxLength(255);

        // Description
        builder.Property(t => t.Description)
 .HasMaxLength(5000);

        // Status - stored as string
     builder.Property(t => t.Status)
            .IsRequired()
        .HasConversion<string>()
 .HasMaxLength(50);

        // Priority - stored as string
        builder.Property(t => t.Priority)
          .IsRequired()
     .HasConversion<string>()
          .HasMaxLength(50);

 // TicketNumber - Value Object (Owned Entity)
  builder.OwnsOne(t => t.TicketNumber, ticketNumber =>
        {
            ticketNumber.Property(tn => tn.Year)
  .HasColumnName("TicketNumber_Year")
        .IsRequired();

    ticketNumber.Property(tn => tn.Sequential)
         .HasColumnName("TicketNumber_Sequential")
         .IsRequired();

        ticketNumber.Property(tn => tn.Value)
    .HasColumnName("TicketNumber")
 .HasMaxLength(50)
     .IsRequired();

  ticketNumber.HasIndex(tn => tn.Value)
  .IsUnique()
    .HasDatabaseName("IX_Tickets_TicketNumber");
        });

        // Foreign Keys
    builder.Property(t => t.OrganizationId)
  .IsRequired();

        builder.HasIndex(t => t.OrganizationId)
     .HasDatabaseName("IX_Tickets_OrganizationId");

        builder.Property(t => t.MachineId)
   .IsRequired();

        builder.HasIndex(t => t.MachineId)
            .HasDatabaseName("IX_Tickets_MachineId");

    builder.Property(t => t.CreatedByUserId)
      .IsRequired();

        builder.HasIndex(t => t.CreatedByUserId)
     .HasDatabaseName("IX_Tickets_CreatedByUserId");

        builder.Property(t => t.AssignedToUserId)
            .IsRequired(false);

        builder.HasIndex(t => t.AssignedToUserId)
      .HasDatabaseName("IX_Tickets_AssignedToUserId");

        // Dates
        builder.Property(t => t.ResolvedAt)
    .IsRequired(false);

        builder.Property(t => t.ClosedAt)
            .IsRequired(false);

        // Auditable fields
        builder.Property(t => t.CreatedAt)
       .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
     .HasDefaultValueSql("NOW()");

        builder.Property(t => t.CreatedBy);
        builder.Property(t => t.UpdatedBy);

        // Indexes for common queries
        builder.HasIndex(t => t.Status)
            .HasDatabaseName("IX_Tickets_Status");

      builder.HasIndex(t => t.Priority)
            .HasDatabaseName("IX_Tickets_Priority");

        builder.HasIndex(t => t.CreatedAt)
   .HasDatabaseName("IX_Tickets_CreatedAt");

  // Ignore domain events collection (not persisted)
      builder.Ignore(t => t.DomainEvents);
    }
}
