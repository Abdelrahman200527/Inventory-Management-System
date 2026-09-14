using System.Text.RegularExpressions;
using AutoMapper;
using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.Entity;
using InventoryManagementSystemDAL.Repo;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SupplierService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Manual validation - there's no FluentValidation in this project, and EF Core
        // does NOT enforce [RegularExpression]/[EmailAddress] data annotations at SaveChanges time,
        // so the BLL has to guard these itself before hitting the database.
        private static void ValidateContactInfo(string phone, string email)
        {
            if (!Regex.IsMatch(phone ?? string.Empty, @"^[0-9]{11}$"))
                throw new BusinessRuleException("Phone must be exactly 11 digits.");

            if (!Regex.IsMatch(email ?? string.Empty, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new BusinessRuleException("Invalid email address format.");
        }

        public async Task<IEnumerable<SupplierResponseDto>> GetAllAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierResponseDto>>(suppliers);
        }

        public async Task<SupplierResponseDto> GetByIdAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Supplier), id);

            return _mapper.Map<SupplierResponseDto>(supplier);
        }

        public async Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto)
        {
            ValidateContactInfo(dto.Phone, dto.Email);

            var duplicates = await _unitOfWork.Suppliers.FindAsync(s => s.Email == dto.Email || s.Phone == dto.Phone);
            if (duplicates.Any())
                throw new BusinessRuleException("A supplier with this email or phone number already exists.");

            var supplier = _mapper.Map<Supplier>(dto);
            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SupplierResponseDto>(supplier);
        }

        public async Task<SupplierResponseDto> UpdateAsync(SupplierUpdateDto dto)
        {
            ValidateContactInfo(dto.Phone, dto.Email);

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.Id)
                ?? throw new NotFoundException(nameof(Supplier), dto.Id);

            // Unique on both Email and Phone - check excluding this record itself
            var duplicates = await _unitOfWork.Suppliers.FindAsync(
                s => (s.Email == dto.Email || s.Phone == dto.Phone) && s.Id != dto.Id);
            if (duplicates.Any())
                throw new BusinessRuleException("Another supplier already uses this email or phone number.");

            _mapper.Map(dto, supplier);
            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SupplierResponseDto>(supplier);
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id, "Purchases")
                ?? throw new NotFoundException(nameof(Supplier), id);

            // Business rule: don't allow deleting a supplier that has purchase history
            if (supplier.Purchases.Any())
                throw new BusinessRuleException("Cannot delete a supplier that has existing purchase records.");

            _unitOfWork.Suppliers.Delete(supplier);
            await _unitOfWork.CompleteAsync();
        }
    }
}