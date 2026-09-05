using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Entity
{
    public class Supplier : BaseEntity
    {
        [Required(ErrorMessage = "Supplier Name is required")]
        public string  SupplierName { get; set; }


        public string ContactName { get; set; }
        [Required(ErrorMessage = "Phone is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone must be 11 digits")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "Phone must be 11 digits")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public string Address { get; set; }

        // Relationships
        public virtual ICollection<Purchase> Purchases { get; set; } = new HashSet<Purchase>();
    }
}
