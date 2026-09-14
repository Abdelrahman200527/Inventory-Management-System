using InventoryManagementSystemDAL.DBContext;
using InventoryManagementSystemDAL.Repo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDAL.Helper
{
    public static class DalService
    {
        public static IServiceCollection DAL(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Add repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // Add Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
