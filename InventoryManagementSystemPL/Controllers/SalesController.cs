using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;

        public SalesController(ISaleService saleService, IProductService productService)
        {
            _saleService = saleService;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var sales = await _saleService.GetAllAsync();
            return View(sales);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Products = await _productService.GetAllAsync();
            return View(new SaleCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                ModelState.AddModelError("", "يجب إضافة صنف واحد على الأقل لعملية البيع.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }

            try
            {
                await _saleService.CreateAsync(dto);
                TempData["Success"] = "تم تسجيل عملية البيع وخصم الكميات من المخزون بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (InsufficientStockException ex)
            {
                // ✅ المخزون مش كفاية - رسالة واضحة
                ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message;
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
            catch (NotFoundException ex)
            {
                // ✅ منتج مش موجود
                ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message;
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
            catch (BusinessRuleException ex)
            {
                // ✅ قواعد العمل
                ModelState.AddModelError("", ex.Message);
                TempData["Error"] = ex.Message;
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
            catch (Exception ex)
            {
                // ✅ أي خطأ تاني غير متوقع
                ModelState.AddModelError("", "حدث خطأ غير متوقع: " + ex.Message);
                TempData["Error"] = "حدث خطأ غير متوقع: " + ex.Message;
                ViewBag.Products = await _productService.GetAllAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null) return NotFound();
            return View(sale);
        }
    }
}