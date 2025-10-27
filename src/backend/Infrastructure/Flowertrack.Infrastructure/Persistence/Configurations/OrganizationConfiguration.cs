using Flowertrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flowertrack.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework configuration for Organization entity
/// </summary>
public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(o => o.Phone)
            .HasMaxLength(50);
        
        builder.Property(o => o.Address)
            .HasMaxLength(500);
        
        builder.Property(o => o.City)
            .HasMaxLength(100);
        
        builder.Property(o => o.PostalCode)
            .HasMaxLength(20);
        
        builder.Property(o => o.Country)
            .HasMaxLength(100);
        
        builder.Property(o => o.ServiceStatus)
            .IsRequired()
            .HasConversion<int>();
        
        builder.Property(o => o.ContractStartDate)
            .IsRequired();
        
        builder.Property(o => o.ContractEndDate)
            .IsRequired(false);
        
        builder.Property(o => o.ApiKey)
            .HasMaxLength(500);
        
        builder.Property(o => o.Notes)
            .HasMaxLength(2000);
        
        builder.Property(o => o.CreatedAt)
            .IsRequired();
        
        builder.Property(o => o.UpdatedAt)
            .IsRequired();
        
        // Indexes
        builder.HasIndex(o => o.Email)
            .IsUnique();
        
        builder.HasIndex(o => o.ServiceStatus);
        
        builder.HasIndex(o => o.ApiKey)
            .IsUnique()
            .HasFilter("[ApiKey] IS NOT NULL");
    }
}
