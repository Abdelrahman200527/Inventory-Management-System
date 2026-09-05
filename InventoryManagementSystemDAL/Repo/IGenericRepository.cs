using System.Linq.Expressions;

namespace InventoryManagementSystemDAL.Repo
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);  
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(params string[] includes);  
        Task<T?> GetByIdAsync(int id, params string[] includes);  
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task SaveChangesAsync();
    }
}