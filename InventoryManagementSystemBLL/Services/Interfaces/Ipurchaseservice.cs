using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemBLL.Services.Interfaces
{
    public interface IPurchaseService
    {
        Task<IEnumerable<PurchaseResponseDto>> GetAllAsync();
        Task<PurchaseResponseDto> GetByIdAsync(int id);

        // Creates the Purchase header + items AND increases stock for every
        // line item, all inside a single Unit of Work commit.
        Task<PurchaseResponseDto> CreateAsync(PurchaseCreateDto dto);
    }
}