using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        IList<EmployeeModel> GetAll();
        EmployeeModel GetById(int id);
        void Create(EmployeeModel employee);
        void Update(EmployeeModel employee);
        void Delete(int id);

    }
}
