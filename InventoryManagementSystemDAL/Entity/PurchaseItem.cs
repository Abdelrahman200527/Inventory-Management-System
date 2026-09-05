using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Entity
{
    public class PurchaseItem: BaseEntity
    {
        [Required(ErrorMessage = "Purchase is required")]
        public int PurchaseId { get; set; }
        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        public int Quantity { get; set; }
        [Required(ErrorMessage = "Unit Cost is required")]
        public decimal UnitCost { get; set; }

        // Relationships
        public virtual Purchase Purchase { get; set; }
        public virtual Product Product { get; set; }
    }
}
