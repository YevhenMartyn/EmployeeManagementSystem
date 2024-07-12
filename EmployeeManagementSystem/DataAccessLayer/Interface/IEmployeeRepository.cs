using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Interface
{
    public interface IEmployeeRepository
    {
        Task<IList<EmployeeEntity>> GetAllAsync(Expression<Func<EmployeeEntity, bool>> filter = null);
        Task<EmployeeEntity> GetByIdAsync(int id);
        Task CreateAsync(EmployeeEntity employee);
        Task UpdateAsync(EmployeeEntity employee);
        Task DeleteAsync(int id);
    }
}
