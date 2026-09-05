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
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.SaleDate).IsRequired().HasDefaultValueSql("GETDATE()");
            builder.Property(s => s.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(s => s.CustomerInfo).HasMaxLength(200);

        }
    }
}
