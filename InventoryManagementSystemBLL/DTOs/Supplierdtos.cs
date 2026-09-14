namespace InventoryManagementSystemBLL.DTOs
{
    public class SupplierCreateDto
    {
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
    }

    public class SupplierUpdateDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
    }

    public class SupplierResponseDto
    {
        public int Id { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Address { get; set; }
    }
}