using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface IInventoryQueryService
    {
        Task<List<ProductResponseDto>> GetLowStockProductsAsync();
        Task<List<ProductResponseDto>> GetOutOfStockProductsAsync();
        Task<List<ProductResponseDto>> SearchProductsByNameAsync(string name);
        Task<ProductResponseDto?> GetProductBySkuAsync(string sku);
        Task<List<SaleResponseDto>> GetRecentSalesAsync(int count);
        Task<List<PurchaseResponseDto>> GetRecentPurchasesAsync(int count);
        Task<List<MostSoldProductDto>> GetMostSoldProductsAsync(int count);
        Task<List<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<List<SupplierResponseDto>> GetAllSuppliersAsync();
        Task<DashboardSummaryDto> GetFullSummaryAsync();
        Task<List<ProductResponseDto>> GetAllProductsListAsync();
    }
}