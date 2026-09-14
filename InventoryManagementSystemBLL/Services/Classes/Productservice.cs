using AutoMapper;
using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.Entity;
using InventoryManagementSystemDAL.Repo;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            // "Category" include so ProductResponseDto.CategoryName isn't empty
            var products = await _unitOfWork.Products.GetAllAsync("Category");
            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        public async Task<ProductResponseDto> GetByIdAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, "Category")
                ?? throw new NotFoundException(nameof(Product), id);

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<PagedResultDto<ProductResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? name = null, int? categoryId = null, bool? isActive = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var products = await _unitOfWork.Products.GetAllAsync("Category");

            // normalize search term
            var term = name?.Trim();

            var filtered = products.Where(p =>
                // search by product name OR SKU when a term is provided
                (string.IsNullOrWhiteSpace(term) ||
                 p.ProductName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                 (!string.IsNullOrWhiteSpace(p.SKU) && p.SKU.Contains(term, StringComparison.OrdinalIgnoreCase))
                ) &&
                // category filter
                (!categoryId.HasValue || p.CategoryId == categoryId.Value) &&
                // active/deleted filter (keeps existing behavior)
                (!isActive.HasValue || p.IsDeleted != isActive.Value))
                .ToList();

            var totalCount = filtered.Count;

            var pageItems = filtered
                .OrderBy(p => p.ProductName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResultDto<ProductResponseDto>
            {
                Items = _mapper.Map<IEnumerable<ProductResponseDto>>(pageItems),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<ProductResponseDto>> GetLowStockProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync("Category");
            var lowStock = products.Where(p => p.StockQuantity <= p.LowStockThreshold);
            return _mapper.Map<IEnumerable<ProductResponseDto>>(lowStock);
        }

        public async Task<ProductResponseDto> CreateAsync(ProductCreateDto dto)
        {
            var skuDuplicate = await _unitOfWork.Products.FindAsync(p => p.SKU == dto.SKU);
            if (skuDuplicate.Any())
                throw new BusinessRuleException($"A product with SKU '{dto.SKU}' already exists.");

            // Make sure the category actually exists before attaching the product to it
            _ = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId)
                ?? throw new NotFoundException(nameof(Category), dto.CategoryId);

            var product = _mapper.Map<Product>(dto);
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task<ProductResponseDto> UpdateAsync(ProductUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException(nameof(Product), dto.Id);

            // Unique index on SKU - check for duplicates excluding this record itself
            var skuDuplicate = await _unitOfWork.Products.FindAsync(p => p.SKU == dto.SKU && p.Id != dto.Id);
            if (skuDuplicate.Any())
                throw new BusinessRuleException($"A product with SKU '{dto.SKU}' already exists.");

            if (product.CategoryId != dto.CategoryId)
            {
                _ = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId)
                    ?? throw new NotFoundException(nameof(Category), dto.CategoryId);
            }

            // Maps everything except StockQuantity (excluded by ProductUpdateDto's shape)
            _mapper.Map(dto, product);

            _unitOfWork.Products.Update(product);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ProductResponseDto>(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Product), id);

            _unitOfWork.Products.Delete(product); // soft delete, handled centrally in GenericRepository
            await _unitOfWork.CompleteAsync();
        }
    }
}