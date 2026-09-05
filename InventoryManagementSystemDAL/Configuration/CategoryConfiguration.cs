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
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category>builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c=> c.Id);
            builder.Property(c=> c.CategoryName).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(700);
            builder.HasIndex(c=> c.CategoryName).IsUnique(); // ==>  ensuring that no two categories can have the same name in the database.
            // Relationship configuration: One Category can have many Products, and each Product belongs to one Category.
            builder.HasMany(c => c.Products).WithOne(p => p.Category).HasForeignKey(p => p.CategoryId).OnDelete(DeleteBehavior.Restrict);

        }
    }
}
