using InventoryManagementSystemDAL.Entity;

namespace InventoryManagementSystemDAL.Repo
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Supplier> Suppliers { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<Purchase> Purchases { get; }
        IGenericRepository<PurchaseItem> PurchaseItems { get; }
        IGenericRepository<Sale> Sales { get; }
        IGenericRepository<SaleItem> SaleItems { get; }

        Task<int> CompleteAsync();
    }
}