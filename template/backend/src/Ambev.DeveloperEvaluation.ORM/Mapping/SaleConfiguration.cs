using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(s => s.SaleDate).IsRequired();
        
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).IsRequired().HasMaxLength(255);
        builder.Property(s => s.CustomerEmail).HasMaxLength(255);
        builder.Property(s => s.CustomerPhone).HasMaxLength(20);
        
        builder.Property(s => s.BranchId).IsRequired();
        builder.Property(s => s.BranchName).IsRequired().HasMaxLength(255);
        builder.Property(s => s.BranchCode).IsRequired().HasMaxLength(50);
        
        builder.Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);
        builder.Property(s => s.CancelledAt);
        builder.Property(s => s.CancelledBy);

        // Unique constraint on SaleNumber
        builder.HasIndex(s => s.SaleNumber).IsUnique();

        // Foreign key relationship with Branch
        builder.HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        builder.HasIndex(s => s.CustomerId);
        builder.HasIndex(s => s.BranchId);
    }
}
