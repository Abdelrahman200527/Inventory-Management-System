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
    public class PurchaseItemConfiguration : IEntityTypeConfiguration<PurchaseItem> 
    {
        public void Configure(EntityTypeBuilder<PurchaseItem> builder)
        {
            builder.ToTable("PurchaseItems");
            builder.HasKey(pi => pi.Id);
            builder.Property(pi => pi.Quantity).IsRequired();
            builder.Property(pi => pi.UnitCost).IsRequired().HasColumnType("decimal(18,2)");
            // Relationships
            builder.HasOne(pi => pi.Purchase).WithMany(p => p.PurchaseItems).HasForeignKey(pi => pi.PurchaseId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(pi => pi.Product).WithMany(p => p.PurchaseItems).HasForeignKey(pi => pi.ProductId).OnDelete(DeleteBehavior.Restrict);
            // Indexes
            builder.HasIndex(pi => new { pi.PurchaseId, pi.ProductId }).IsUnique();
        }
    }
}
