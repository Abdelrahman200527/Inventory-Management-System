namespace InventoryManagementSystemBLL.DTOs
{
    public class RecentActivityDto
    {
        public string Type { get; set; } = string.Empty; // "Sale" or "Purchase"
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class MostSoldProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class DashboardSummaryDto
    {
        // Key system metrics (spec section 3)
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalStockQuantity { get; set; }
        public int LowStockProductsCount { get; set; }

        public int TotalPurchasesCount { get; set; }
        public decimal TotalPurchasesAmount { get; set; }

        public int TotalSalesCount { get; set; }
        public decimal TotalSalesAmount { get; set; }

        public List<RecentActivityDto> RecentActivity { get; set; } = new();
        public List<MostSoldProductDto> MostSoldProducts { get; set; } = new();
    }
}