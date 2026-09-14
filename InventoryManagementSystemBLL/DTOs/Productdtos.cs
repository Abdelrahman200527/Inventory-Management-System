namespace InventoryManagementSystemBLL.DTOs
{
    public class ProductCreateDto
    {
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; } = 3;
    }

    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public decimal UnitPrice { get; set; }
        public int LowStockThreshold { get; set; }
        // StockQuantity intentionally excluded - it should only change via
        // Purchases (increase) and Sales (decrease), never a manual edit.
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int LowStockThreshold { get; set; }
        public bool IsLowStock => StockQuantity <= LowStockThreshold;
    }
}