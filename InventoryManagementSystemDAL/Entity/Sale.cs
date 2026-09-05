using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Entity
{
    public class Sale: BaseEntity
    {
        public DateTime SaleDate { get; set; } = DateTime.Now;
        [Required (ErrorMessage = "Total Amount is required")]
        public decimal TotalAmount { get; set; }
        public string CustomerInfo { get; set; }

        // Relationships
        public virtual ICollection<SaleItem> SaleItems { get; set; } = new HashSet<SaleItem>();
    }
}
