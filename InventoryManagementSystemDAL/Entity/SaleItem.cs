using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Entity
{
    public class SaleItem: BaseEntity
    {
        [Required(ErrorMessage = "Sale is required")]
        public int SaleId { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Unit Price is required")]
        public decimal UnitPrice { get; set; }
        // Relationships
        public virtual Sale Sale { get; set; }
        public virtual Product Product { get; set; }
    
    }
}
