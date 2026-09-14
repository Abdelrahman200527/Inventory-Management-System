using AutoMapper;
using InventoryManagementSystemBLL.DTOs;
using InventoryManagementSystemDAL.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InventoryManagementSystemBLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ---------- Category ----------
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

            // ---------- Supplier ----------
            CreateMap<SupplierCreateDto, Supplier>();
            CreateMap<SupplierUpdateDto, Supplier>();
            CreateMap<Supplier, SupplierResponseDto>();

            // ---------- Product ----------
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>(); // StockQuantity absent from source DTO, so it's left untouched
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty));

            // ---------- Purchase ----------
            CreateMap<Purchase, PurchaseResponseDto>()
                .ForMember(dest => dest.SupplierName,
                           opt => opt.MapFrom(src => src.Supplier != null ? src.Supplier.SupplierName : string.Empty))
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.PurchaseItems));

            CreateMap<PurchaseItem, PurchaseItemResponseDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));

            // ---------- Sale ----------
            CreateMap<Sale, SaleResponseDto>()
                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.SaleItems));

            CreateMap<SaleItem, SaleItemResponseDto>()
                .ForMember(dest => dest.ProductName,
                           opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));
        }
    }
}