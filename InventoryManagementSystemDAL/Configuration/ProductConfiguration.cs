using InventoryManagementSystemDAL.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(50);
            builder.Property(p => p.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(p => p.StockQuantity).IsRequired();
            builder.Property(p => p.LowStockThreshold).IsRequired().HasDefaultValue(3);
            //relationships
            builder.HasOne(p=> p.Category).WithMany(c=> c.Products).HasForeignKey(p => p.CategoryId) .OnDelete(DeleteBehavior.Restrict);
            // Indexes
            builder.HasIndex(p => p.SKU).IsUnique();
            builder.HasIndex(p => p.ProductName);
            builder.HasIndex(p => p.CategoryId);

        }
    }
}
