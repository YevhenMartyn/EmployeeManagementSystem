using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        IList<Employee> GetAll();
        Employee GetById(int id);
        void Create(Employee employee);
        void Update(Employee employee);
        void Delete(int id);

    }
}
