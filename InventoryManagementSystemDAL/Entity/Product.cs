using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Entity
{
    public class Product: BaseEntity
    {
        [Required(ErrorMessage = "SKU is required")]
        public string SKU { get; set; }
        [Required(ErrorMessage = "Product Name is required")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Unit Price is required")]
        public decimal UnitPrice { get; set; }
        [Required(ErrorMessage = "Stock Quantity is required")]
        public int StockQuantity { get; set; }
        [Required(ErrorMessage = "Low Stock Threshold is required")]
        public int LowStockThreshold { get; set; } = 3;
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        // Relationships
        public virtual Category Category { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new HashSet<PurchaseItem>();

        public virtual ICollection<SaleItem> SaleItems { get; set; } = new HashSet<SaleItem>();
    }
}
