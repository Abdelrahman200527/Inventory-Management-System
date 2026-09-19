using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Services.Interfaces;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class InventoryQueryService : IInventoryQueryService
    {
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;
        private readonly IPurchaseService _purchaseService;
        private readonly IDashboardService _dashboardService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;

        public InventoryQueryService(
            IProductService productService,
            ISaleService saleService,
            IPurchaseService purchaseService,
            IDashboardService dashboardService,
            ICategoryService categoryService,
            ISupplierService supplierService)
        {
            _productService = productService;
            _saleService = saleService;
            _purchaseService = purchaseService;
            _dashboardService = dashboardService;
            _categoryService = categoryService;
            _supplierService = supplierService;
        }

        public async Task<List<ProductResponseDto>> GetLowStockProductsAsync()
        {
            var all = await _productService.GetAllAsync();
            return all.Where(p => p.StockQuantity <= p.LowStockThreshold).ToList();
        }

        public async Task<List<ProductResponseDto>> GetOutOfStockProductsAsync()
        {
            var all = await _productService.GetAllAsync();
            return all.Where(p => p.StockQuantity == 0).ToList();
        }

        public async Task<List<ProductResponseDto>> SearchProductsByNameAsync(string name)
        {
            var all = await _productService.GetAllAsync();
            return all.Where(p => p.ProductName.Contains(name, StringComparison.OrdinalIgnoreCase)
                              || p.SKU.Contains(name, StringComparison.OrdinalIgnoreCase))
                      .ToList();
        }

        public async Task<ProductResponseDto?> GetProductBySkuAsync(string sku)
        {
            var all = await _productService.GetAllAsync();
            return all.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<SaleResponseDto>> GetRecentSalesAsync(int count)
            => (await _saleService.GetAllAsync()).OrderByDescending(s => s.SaleDate).Take(count).ToList();

        public async Task<List<PurchaseResponseDto>> GetRecentPurchasesAsync(int count)
            => (await _purchaseService.GetAllAsync()).OrderByDescending(p => p.PurchaseDate).Take(count).ToList();

        public async Task<List<MostSoldProductDto>> GetMostSoldProductsAsync(int count)
        {
            var summary = await _dashboardService.GetSummaryAsync(0, count);
            return summary.MostSoldProducts;
        }

        public async Task<List<CategoryResponseDto>> GetAllCategoriesAsync()
            => (await _categoryService.GetAllAsync()).ToList();

        public async Task<List<SupplierResponseDto>> GetAllSuppliersAsync()
            => (await _supplierService.GetAllAsync()).ToList();

        public async Task<DashboardSummaryDto> GetFullSummaryAsync()
            => await _dashboardService.GetSummaryAsync(10, 5);

        public async Task<List<ProductResponseDto>> GetAllProductsListAsync()
            => (await _productService.GetAllAsync()).ToList();
    }
}