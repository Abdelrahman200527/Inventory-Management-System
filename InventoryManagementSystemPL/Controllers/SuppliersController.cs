using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IPurchaseService _purchaseService;

        public SuppliersController(ISupplierService supplierService, IPurchaseService purchaseService)
        {
            _supplierService = supplierService;
            _purchaseService = purchaseService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return View(suppliers);
        }

        public IActionResult Create() => View(new SupplierCreateDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _supplierService.CreateAsync(dto);
                TempData["Success"] = "تم إضافة المورد بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var s = await _supplierService.GetByIdAsync(id);
            if (s == null) return NotFound();
            var dto = new SupplierUpdateDto
            {
                Id = s.Id,
                SupplierName = s.SupplierName,
                ContactName = s.ContactName,
                Phone = s.Phone,
                Email = s.Email,
                Address = s.Address
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _supplierService.UpdateAsync(dto);
                TempData["Success"] = "تم تعديل بيانات المورد بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var s = await _supplierService.GetByIdAsync(id);
            if (s == null) return NotFound();
            var allPurchases = await _purchaseService.GetAllAsync();
            ViewBag.Purchases = allPurchases.Where(p => p.SupplierId == id).ToList();
            return View(s);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var s = await _supplierService.GetByIdAsync(id);
            if (s == null) return NotFound();
            return View(s);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _supplierService.DeleteAsync(id);
                TempData["Success"] = "تم حذف المورد بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
