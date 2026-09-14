using AutoMapper;
using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.Entity;
using InventoryManagementSystemDAL.Repo;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PurchaseResponseDto>> GetAllAsync()
        {
            var purchases = await _unitOfWork.Purchases.GetAllAsync("Supplier", "PurchaseItems", "PurchaseItems.Product");
            return _mapper.Map<IEnumerable<PurchaseResponseDto>>(purchases);
        }

        public async Task<PurchaseResponseDto> GetByIdAsync(int id)
        {
            var purchase = await _unitOfWork.Purchases.GetByIdAsync(id, "Supplier", "PurchaseItems", "PurchaseItems.Product")
                ?? throw new NotFoundException(nameof(Purchase), id);

            return _mapper.Map<PurchaseResponseDto>(purchase);
        }

        public async Task<PurchaseResponseDto> CreateAsync(PurchaseCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new BusinessRuleException("A purchase must contain at least one item.");

            // Unique index on (PurchaseId, ProductId) means the same product can't appear
            // twice as separate line items in one purchase - reject early with a clear message
            // instead of letting the database throw a constraint violation.
            var duplicateProductIds = dto.Items
                .GroupBy(i => i.ProductId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateProductIds.Any())
                throw new BusinessRuleException(
                    "The same product cannot appear twice in one purchase. Combine the quantities into a single line item instead.");

            _ = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId)
                ?? throw new NotFoundException(nameof(Supplier), dto.SupplierId);

            var purchase = new Purchase
            {
                SupplierId = dto.SupplierId,
                PurchaseDate = DateTime.Now,
                TotalAmount = 0
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                    ?? throw new NotFoundException(nameof(Product), item.ProductId);

                if (item.Quantity <= 0)
                    throw new BusinessRuleException($"Quantity for '{product.ProductName}' must be greater than zero.");

                var lineTotal = item.UnitCost * item.Quantity;
                total += lineTotal;

                purchase.PurchaseItems.Add(new PurchaseItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitCost = item.UnitCost
                });

                // Purchasing INCREASES stock - the opposite of what SaleService does
                product.StockQuantity += item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            purchase.TotalAmount = total;
            await _unitOfWork.Purchases.AddAsync(purchase);

            // One commit: Purchase header + items + every product's new stock, together
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<PurchaseResponseDto>(purchase);
        }
    }
}