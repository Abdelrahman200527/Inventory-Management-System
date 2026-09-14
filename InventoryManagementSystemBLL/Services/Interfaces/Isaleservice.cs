using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface ISaleService
    {
        Task<IEnumerable<SaleResponseDto>> GetAllAsync();
        Task<SaleResponseDto> GetByIdAsync(int id);

        // Creates the Sale header + items AND deducts stock for every line
        // item, all inside a single Unit of Work commit.
        Task<SaleResponseDto> CreateAsync(SaleCreateDto dto);
    }
}