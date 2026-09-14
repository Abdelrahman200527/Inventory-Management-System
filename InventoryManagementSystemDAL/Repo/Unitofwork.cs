using InventoryManagementSystemDAL.DBContext;
using InventoryManagementSystemDAL.Entity;

namespace InventoryManagementSystemDAL.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryDbContext _context;
        private bool _disposed;

        private IGenericRepository<Category>? _categories;
        private IGenericRepository<Supplier>? _suppliers;
        private IGenericRepository<Product>? _products;
        private IGenericRepository<Purchase>? _purchases;
        private IGenericRepository<PurchaseItem>? _purchaseItems;
        private IGenericRepository<Sale>? _sales;
        private IGenericRepository<SaleItem>? _saleItems;

        public UnitOfWork(InventoryDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Category> Categories => _categories ??= new GenericRepository<Category>(_context);
        public IGenericRepository<Supplier> Suppliers => _suppliers ??= new GenericRepository<Supplier>(_context);
        public IGenericRepository<Product> Products => _products ??= new GenericRepository<Product>(_context);
        public IGenericRepository<Purchase> Purchases => _purchases ??= new GenericRepository<Purchase>(_context);
        public IGenericRepository<PurchaseItem> PurchaseItems => _purchaseItems ??= new GenericRepository<PurchaseItem>(_context);
        public IGenericRepository<Sale> Sales => _sales ??= new GenericRepository<Sale>(_context);
        public IGenericRepository<SaleItem> SaleItems => _saleItems ??= new GenericRepository<SaleItem>(_context);

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose()
        {
            if (!_disposed)
            {
                _context.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}