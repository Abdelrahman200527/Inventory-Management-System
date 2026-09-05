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
    public class SupplierConfiguration: IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.SupplierName).IsRequired().HasMaxLength(100);

            builder.Property(s => s.ContactName).HasMaxLength(100);
            builder.Property(s => s.Phone).IsRequired().HasMaxLength(11).IsFixedLength().HasColumnType("char(11)");

            builder.Property(s => s.Email).HasMaxLength(100);
            builder.Property(s => s.Address).HasMaxLength(200);


            builder.HasIndex(s=> s.Phone).IsUnique();
            builder.HasIndex(s => s.Email).IsUnique();

        }
    }
}
