using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierResponseDto>> GetAllAsync();
        Task<SupplierResponseDto> GetByIdAsync(int id);
        Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto);
        Task<SupplierResponseDto> UpdateAsync(SupplierUpdateDto dto);
        Task DeleteAsync(int id);
    }
}