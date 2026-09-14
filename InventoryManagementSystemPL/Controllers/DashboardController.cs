using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IProductService _productService;

        public DashboardController(IDashboardService dashboardService, IProductService productService)
        {
            _dashboardService = dashboardService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var summary = await _dashboardService.GetSummaryAsync(recentActivityCount: 10, topProductsCount: 6);
            return View(summary);
        }
    }
}
