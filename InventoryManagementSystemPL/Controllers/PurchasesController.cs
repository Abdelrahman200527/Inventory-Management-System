using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;

        public PurchasesController(IPurchaseService purchaseService, ISupplierService supplierService, IProductService productService)
        {
            _purchaseService = purchaseService;
            _supplierService = supplierService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var purchases = await _purchaseService.GetAllAsync();
            return View(purchases);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Suppliers = await _supplierService.GetAllAsync();
            ViewBag.Products = await _productService.GetAllAsync();
            return View(new PurchaseCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                ModelState.AddModelError("", "يجب إضافة صنف واحد على الأقل لفاتورة الشراء.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Suppliers = await _supplierService.GetAllAsync();
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }

            try
            {
                await _purchaseService.CreateAsync(dto);
                TempData["Success"] = "تم تسجيل عملية الشراء وتحديث المخزون بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (NotFoundException ex)
            {
                ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message;
                ViewBag.Suppliers = await _supplierService.GetAllAsync();
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message;
                ViewBag.Suppliers = await _supplierService.GetAllAsync();
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "حدث خطأ غير متوقع: " + ex.Message);
                TempData["Error"] = "حدث خطأ غير متوقع: " + ex.Message;
                ViewBag.Suppliers = await _supplierService.GetAllAsync();
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _purchaseService.GetByIdAsync(id);
            if (purchase == null) return NotFound();
            return View(purchase);
        }
    }
}