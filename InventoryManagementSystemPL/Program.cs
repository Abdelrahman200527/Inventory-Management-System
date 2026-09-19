using InventoryManagementSystemBLL.Mapping;
using InventoryManagementSystemBLL.Services.Classes;
using InventoryManagementSystemBLL.Services.Interfaces;
using InventoryManagementSystemDAL.DBContext;
using InventoryManagementSystemDAL.Helper;
using InventoryManagementSystemBLL.Helper;
using InventoryManagementSystemDAL.Repo;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

          builder.Services.DAL(builder.Configuration).BLL();  // Call All Service in Dal + Bll



            builder.Services.AddScoped<IInventoryQueryService, InventoryQueryService>();
            builder.Services.AddHttpClient<IAiChatService, GeminiChatService>();


            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}