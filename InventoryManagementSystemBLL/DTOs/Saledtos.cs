namespace InventoryManagementSystemBLL.DTOs
{
    public class SaleItemCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        // UnitPrice is NOT accepted from the client - the service reads the
        // product's current price itself so it can't be tampered with.
    }

    public class SaleCreateDto
    {
        public string? CustomerInfo { get; set; }
        public List<SaleItemCreateDto> Items { get; set; } = new();
    }

    public class SaleItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;
    }

    public class SaleResponseDto
    {
        public int Id { get; set; }
        public DateTime SaleDate { get; set; }
        public string? CustomerInfo { get; set; }
        public decimal TotalAmount { get; set; }
        public List<SaleItemResponseDto> Items { get; set; } = new();
    }
}