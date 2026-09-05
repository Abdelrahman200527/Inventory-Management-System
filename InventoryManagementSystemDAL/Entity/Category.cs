using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Entity
{
    public class Category: BaseEntity
    {
        [Required(ErrorMessage = "Category Name is required")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        // Relationships
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
