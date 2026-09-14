using InventoryManagementSystemBLL.DTOs;

namespace InventoryManagementSystemPL.Models
{
    public class ProductIndexViewModel
    {
        public PagedResultDto<ProductResponseDto> Products { get; set; } = new();
        public IEnumerable<CategoryResponseDto> Categories { get; set; } = new List<CategoryResponseDto>();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public string? StockStatus { get; set; }
    }

    public class PurchaseCreateViewModel
    {
        public int SupplierId { get; set; }
        public List<PurchaseItemCreateDto> Items { get; set; } = new();
    }

    public class SaleCreateViewModel
    {
        public string? CustomerInfo { get; set; }
        public List<SaleItemCreateDto> Items { get; set; } = new();
    }

    public class DeleteConfirmViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int RelatedCount { get; set; }
        public bool CanDelete => RelatedCount == 0;
        public string? Reason { get; set; }
    }
}
