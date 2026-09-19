namespace InventoryManagementSystemBLL.DTOs
{
    public class CategoryCreateDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; } 
    }

    public class CategoryUpdateDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } =string.Empty; 
        public string? Description { get; set; } 
    }

    public class CategoryResponseDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int ProductCount { get; set; }
    }
}