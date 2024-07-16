using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        Task<IList<EmployeeModel>> GetAllAsync(EmployeeFilterModel? filter);
        Task<EmployeeModel> GetByIdAsync(int id);
        Task CreateAsync(EmployeeModel employee);
        Task UpdateAsync(int id, EmployeeModel employee);
        Task DeleteAsync(int id);
    }
}
