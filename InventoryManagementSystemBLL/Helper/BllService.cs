using InventoryManagementSystemBLL.Mapping;
using InventoryManagementSystemBLL.Services.Classes;
using InventoryManagementSystemBLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemBLL.Helper
{
    public static class BllService
    {
        public static IServiceCollection BLL(this IServiceCollection Services)
        {
            Services.AddScoped<ICategoryService, CategoryService>();
            Services.AddScoped<IDashboardService, DashboardService>();
            Services.AddScoped<IProductService, ProductService>();
            Services.AddScoped<IPurchaseService, PurchaseService>();
            Services.AddScoped<ISaleService, SaleService>();
            Services.AddAutoMapper(M => M.AddProfile(new MappingProfile()));
            Services.AddScoped<ISupplierService, SupplierService>();
            return Services;
        }
    }
}
