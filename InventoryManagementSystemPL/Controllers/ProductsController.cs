using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemPL.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? searchTerm, int? categoryId, string? stockStatus, int page = 1)
        {
            var paged = await _productService.GetPagedAsync(page, 10, searchTerm, categoryId);
            var categories = await _categoryService.GetAllAsync();

            var vm = new ProductIndexViewModel
            {
                Products = paged,
                Categories = categories,
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                StockStatus = stockStatus
            };
            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _categoryService.GetAllAsync();
            return View(new ProductCreateDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(dto);
            }
            try
            {
                await _productService.CreateAsync(dto);
                TempData["Success"] = "تم إضافة المنتج بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var p = await _productService.GetByIdAsync(id);
            if (p == null) return NotFound();
            ViewBag.Categories = await _categoryService.GetAllAsync();
            var dto = new ProductUpdateDto
            {
                Id = p.Id,
                SKU = p.SKU,
                ProductName = p.ProductName,
                CategoryId = p.CategoryId,
                UnitPrice = p.UnitPrice,
                LowStockThreshold = p.LowStockThreshold
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(dto);
            }
            try
            {
                await _productService.UpdateAsync(dto);
                TempData["Success"] = "تم تعديل المنتج بنجاح!";
                return RedirectToAction(nameof(Index));
            }
            catch (BusinessRuleException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Categories = await _categoryService.GetAllAsync();
                return View(dto);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var p = await _productService.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var p = await _productService.GetByIdAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                TempData["Success"] = "تم حذف المنتج بنجاح!";
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
