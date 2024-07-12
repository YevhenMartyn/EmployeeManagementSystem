using System.Linq.Expressions;

namespace DataAccessLayer.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> filter = null);
        Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
    }
}