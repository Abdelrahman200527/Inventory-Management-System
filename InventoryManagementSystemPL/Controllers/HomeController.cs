using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => RedirectToAction("Index", "Dashboard");
    }
}
