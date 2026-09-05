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
    public class SaleItemConfiguration: IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.ToTable("SaleItems");
            builder.HasKey(si=> si.Id);

            builder.Property(si => si.Quantity).IsRequired();
            builder.Property(si=> si.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");

            // Relationships
            builder.HasOne(si=> si.Sale).WithMany(s=> s.SaleItems).HasForeignKey(si => si.SaleId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(si=> si.Product).WithMany(p=>p.SaleItems).HasForeignKey(si=> si.ProductId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
