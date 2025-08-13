using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);
        builder.Property(si => si.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(si => si.SaleId).IsRequired();
        
        builder.Property(si => si.ProductId).IsRequired();
        builder.Property(si => si.ProductName).IsRequired().HasMaxLength(255);
        builder.Property(si => si.ProductCode).HasMaxLength(100);
        builder.Property(si => si.ProductDescription).HasColumnType("text");
        
        builder.Property(si => si.Quantity).IsRequired();
        builder.Property(si => si.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(si => si.DiscountPercentage).IsRequired().HasColumnType("decimal(5,2)");
        builder.Property(si => si.DiscountAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(si => si.TotalItemAmount).IsRequired().HasColumnType("decimal(18,2)");

        builder.Property(si => si.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(si => si.CreatedAt).IsRequired();
        builder.Property(si => si.UpdatedAt);
        builder.Property(si => si.CancelledAt);
        builder.Property(si => si.CancelledBy);

        // Foreign key relationship with Sale
        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(si => si.SaleId);
        builder.HasIndex(si => si.ProductId);
    }
}
