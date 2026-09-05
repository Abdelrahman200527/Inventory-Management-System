using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace InventoryManagementSystemDAL.Entity
{
    public class Purchase : BaseEntity
    {
        [Required(ErrorMessage = "Supplier is required")]
        public int SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Total Amount is required")]
        public decimal TotalAmount { get; set; }

        // Relationships
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<PurchaseItem> PurchaseItems { get; set; } = new HashSet<PurchaseItem>();
    }
}
