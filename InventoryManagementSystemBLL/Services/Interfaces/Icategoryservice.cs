using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);
        Task<CategoryResponseDto> UpdateAsync(CategoryUpdateDto dto);
        Task DeleteAsync(int id);
    }
}