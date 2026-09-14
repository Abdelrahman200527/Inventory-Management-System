using AutoMapper;
using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.Entity;
using InventoryManagementSystemDAL.Repo;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync("Products");
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto> GetByIdAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, "Products")
                ?? throw new NotFoundException(nameof(Category), id);

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
        {
            var duplicate = await _unitOfWork.Categories.FindAsync(c => c.CategoryName == dto.CategoryName);
            if (duplicate.Any())
                throw new BusinessRuleException($"Category '{dto.CategoryName}' already exists.");

            var category = _mapper.Map<Category>(dto);
            category.Description = dto.Description ?? string.Empty;
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> UpdateAsync(CategoryUpdateDto dto)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException(nameof(Category), dto.Id);

            // Unique index on CategoryName - check for duplicates excluding this record itself
            var duplicate = await _unitOfWork.Categories.FindAsync(c => c.CategoryName == dto.CategoryName && c.Id != dto.Id);
            if (duplicate.Any())
                throw new BusinessRuleException($"Category '{dto.CategoryName}' already exists.");

            _mapper.Map(dto, category);
            category.Description = dto.Description ?? string.Empty;
            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, "Products")
                ?? throw new NotFoundException(nameof(Category), id);

            // Business rule: don't allow deleting a category that still has products in it
            if (category.Products.Any())
                throw new BusinessRuleException("Cannot delete a category that still has products assigned to it.");

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.CompleteAsync();
        }
    }
}