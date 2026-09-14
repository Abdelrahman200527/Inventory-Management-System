using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystemPL.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create() => View(new CategoryCreateDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _categoryService.CreateAsync(dto);
                TempData["Success"] = "تم إضافة القسم بنجاح!";
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
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            var dto = new CategoryUpdateDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            try
            {
                await _categoryService.UpdateAsync(dto);
                TempData["Success"] = "تم تعديل القسم بنجاح!";
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
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                TempData["Success"] = "تم حذف القسم بنجاح!";
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
