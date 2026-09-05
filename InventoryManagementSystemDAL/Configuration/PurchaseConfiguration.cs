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
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {

        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");

            builder.Property(p => p.PurchaseDate).IsRequired().HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(p => p.Supplier).WithMany(s => s.Purchases).HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
