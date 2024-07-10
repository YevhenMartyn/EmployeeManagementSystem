using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Interface
{
    public interface IEmployeeRepository
    {
        List<Employee> GetAll(Expression<Func<Employee, bool>> filter = null);
        Employee GetById(int id);
        void Create(Employee employee);
        void Update(Employee employee);
        void Delete(int id);
        void SaveChanges();
        
    }
}
