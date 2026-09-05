using InventoryManagementSystemDAL.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace InventoryManagementSystemDAL.Repo
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly InventoryDbContext DB;
        protected readonly DbSet<T> dbSet;

        public GenericRepository(InventoryDbContext dbContext)
        {
            DB = dbContext;
            dbSet = DB.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id) => await dbSet.FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync()=> await dbSet.ToListAsync();

        public async Task<IEnumerable<T>> GetAllAsync(params string[] includes)
        {
            var query = dbSet.AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return await query.ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id, params string[] includes)
        {
            var query = dbSet.AsQueryable();
            foreach (var include in includes)
                query = query.Include(include);
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)=> await dbSet.Where(predicate).ToListAsync();

        public async Task AddAsync(T entity) => await dbSet.AddAsync(entity);

        public void Update(T entity)=> dbSet.Update(entity);

        public void Delete(T entity)=> dbSet.Remove(entity);

        public async Task SaveChangesAsync()=> await DB.SaveChangesAsync();
    }
}