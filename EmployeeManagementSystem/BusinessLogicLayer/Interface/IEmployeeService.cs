using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        IList<EmployeeModel> GetAll();
        IList<EmployeeModel> GetAll(int? departmentId, DateTime? fromDate, DateTime? toDate);
        IList<EmployeeModel> GetAll(string? name, string? position, int? departmentId, DateTime? startDate);
        EmployeeModel GetById(int id);
        void Create(EmployeeModel employee);
        void Update(int id, EmployeeModel employee);
        void Delete(int id);

    }
}
