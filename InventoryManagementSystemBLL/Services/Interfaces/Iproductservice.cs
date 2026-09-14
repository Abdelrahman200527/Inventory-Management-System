using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto> GetByIdAsync(int id);
        Task<PagedResultDto<ProductResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? name = null, int? categoryId = null, bool? isActive = null);
        Task<ProductResponseDto> CreateAsync(ProductCreateDto dto);
        Task<ProductResponseDto> UpdateAsync(ProductUpdateDto dto);
        Task DeleteAsync(int id);
    }
}