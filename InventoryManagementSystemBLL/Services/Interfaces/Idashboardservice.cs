using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface IDashboardService
    {
        // Everything the dashboard page needs in one call
        Task<DashboardSummaryDto> GetSummaryAsync(int recentActivityCount = 10, int topProductsCount = 5);

        // Exposed separately too, in case the PL wants to refresh just one widget
        // (e.g. an AJAX call that reloads only "Most Sold Products") without refetching everything.
        Task<IEnumerable<RecentActivityDto>> GetRecentActivityAsync(int count = 10);
        Task<IEnumerable<MostSoldProductDto>> GetMostSoldProductsAsync(int count = 5);
    }
}