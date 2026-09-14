using AutoMapper;
using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemBLL.Exceptions;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.Entity;
using InventoryManagementSystemDAL.Repo;

namespace InventoryManagementSystemBLL.Services.Classes
{
    public class SaleService : ISaleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SaleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SaleResponseDto>> GetAllAsync()
        {
            var sales = await _unitOfWork.Sales.GetAllAsync("SaleItems", "SaleItems.Product");
            return _mapper.Map<IEnumerable<SaleResponseDto>>(sales);
        }

        public async Task<SaleResponseDto> GetByIdAsync(int id)
        {
            var sale = await _unitOfWork.Sales.GetByIdAsync(id, "SaleItems", "SaleItems.Product")
                ?? throw new NotFoundException(nameof(Sale), id);

            return _mapper.Map<SaleResponseDto>(sale);
        }

        public async Task<SaleResponseDto> CreateAsync(SaleCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new BusinessRuleException("A sale must contain at least one item.");

            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                CustomerInfo = dto.CustomerInfo,
                TotalAmount = 0
            };

            decimal total = 0;

            foreach (var item in dto.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId)
                    ?? throw new NotFoundException(nameof(Product), item.ProductId);

                // 1) Confirm enough stock exists BEFORE touching anything
                if (product.StockQuantity < item.Quantity)
                    throw new InsufficientStockException(product.ProductName, item.Quantity, product.StockQuantity);

                // 2) Snapshot the current price - never trust a price sent from the client
                var lineTotal = product.UnitPrice * item.Quantity;
                total += lineTotal;

                sale.SaleItems.Add(new SaleItem
                {
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = product.UnitPrice
                });

                // 3) Deduct stock (still only in memory - nothing saved yet)
                product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(product);
            }

            sale.TotalAmount = total;
            await _unitOfWork.Sales.AddAsync(sale);

            // 4) ONE SaveChanges commits the Sale, its SaleItems, and every
            //    product's new StockQuantity together. If anything above threw,
            //    nothing here gets saved.
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<SaleResponseDto>(sale);
        }
    }
}