using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.Interface
{
    public interface IEmployeeRepository
    {
        IList<EmployeeEntity> GetAll(Expression<Func<EmployeeEntity, bool>> filter = null);
        EmployeeEntity GetById(int id);
        void Create(EmployeeEntity employee);
        void Update(EmployeeEntity employee);
        void Delete(int id);

    }
}
