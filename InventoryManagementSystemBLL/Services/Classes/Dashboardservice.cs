using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.Repo;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync(int recentActivityCount = 10, int topProductsCount = 5)
        {
            // NOTE: GetAllAsync() loads everything into memory then we aggregate with LINQ,
            // because GenericRepository doesn't expose SQL-side aggregates (Sum/Count/GroupBy
            // translated to the database). Fine for a project of this size; if the data grows
            // large, this is the first place to optimize (e.g. add dedicated count/sum queries
            // to IGenericRepository).
            var products = (await _unitOfWork.Products.GetAllAsync()).ToList();
            var categories = (await _unitOfWork.Categories.GetAllAsync()).ToList();
            var suppliers = (await _unitOfWork.Suppliers.GetAllAsync()).ToList();
            var purchases = (await _unitOfWork.Purchases.GetAllAsync("Supplier")).ToList();
            var sales = (await _unitOfWork.Sales.GetAllAsync("SaleItems", "SaleItems.Product")).ToList();

            var summary = new DashboardSummaryDto
            {
                TotalProducts = products.Count,
                TotalCategories = categories.Count,
                TotalSuppliers = suppliers.Count,
                TotalStockQuantity = products.Sum(p => p.StockQuantity),
                LowStockProductsCount = products.Count(p => p.StockQuantity <= p.LowStockThreshold),

                TotalPurchasesCount = purchases.Count,
                TotalPurchasesAmount = purchases.Sum(p => p.TotalAmount),

                TotalSalesCount = sales.Count,
                TotalSalesAmount = sales.Sum(s => s.TotalAmount)
            };

            summary.RecentActivity = BuildRecentActivity(sales, purchases, recentActivityCount);
            summary.MostSoldProducts = BuildMostSoldProducts(sales, topProductsCount);

            return summary;
        }

        public async Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(int count = 10)
        {
            var purchases = (await _unitOfWork.Purchases.GetAllAsync("Supplier")).ToList();
            var sales = (await _unitOfWork.Sales.GetAllAsync()).ToList();

            return BuildRecentActivity(sales, purchases, count);
        }

        public async Task<IEnumerable<MostSoldProductDto>> GetMostSoldProductsAsync(int count = 5)
        {
            var sales = (await _unitOfWork.Sales.GetAllAsync("SaleItems", "SaleItems.Product")).ToList();
            return BuildMostSoldProducts(sales, count);
        }

        // ---------- private helpers ----------

        private static List<RecentActivityDto> BuildRecentActivity(
            IEnumerable<InventoryManagementSystemDAL.Entity.Sale> sales,
            IEnumerable<InventoryManagementSystemDAL.Entity.Purchase> purchases,
            int count)
        {
            var activity = new List<RecentActivityDto>();

            activity.AddRange(sales.Select(s => new RecentActivityDto
            {
                Type = "Sale",
                Description = string.IsNullOrWhiteSpace(s.CustomerInfo) ? "Sale" : $"Sale to {s.CustomerInfo}",
                Date = s.SaleDate,
                Amount = s.TotalAmount
            }));

            activity.AddRange(purchases.Select(p => new RecentActivityDto
            {
                Type = "Purchase",
                Description = p.Supplier != null ? $"Purchase from {p.Supplier.SupplierName}" : "Purchase",
                Date = p.PurchaseDate,
                Amount = p.TotalAmount
            }));

            return activity
                .OrderByDescending(a => a.Date)
                .Take(count)
                .ToList();
        }

        private static List<MostSoldProductDto> BuildMostSoldProducts(
            IEnumerable<InventoryManagementSystemDAL.Entity.Sale> sales,
            int count)
        {
            return sales
                .SelectMany(s => s.SaleItems)
                .GroupBy(si => new
                {
                    si.ProductId,
                    ProductName = si.Product != null ? si.Product.ProductName : "Unknown"
                })
                .Select(g => new MostSoldProductDto
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantitySold = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.Quantity * x.UnitPrice)
                })
                .OrderByDescending(x => x.TotalQuantitySold)
                .Take(count)
                .ToList();
        }
    }
}