namespace InventoryManagementSystemBLL.DTOs
{
    public class PurchaseItemCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
    }

    public class PurchaseCreateDto
    {
        public int SupplierId { get; set; }
        public List<PurchaseItemCreateDto> Items { get; set; } = new();
    }

    public class PurchaseItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal => Quantity * UnitCost;
    }

    public class PurchaseResponseDto
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<PurchaseItemResponseDto> Items { get; set; } = new();
    }
}