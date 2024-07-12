using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IEmployeeService
    {
        Task<IList<EmployeeModel>> GetAllAsync();
        Task<IList<EmployeeModel>> GetAllAsync(int? departmentId, DateTime? fromDate, DateTime? toDate);
        Task<IList<EmployeeModel>> GetAllAsync(string? name, string? position, int? departmentId, DateTime? startDate);
        Task<EmployeeModel> GetByIdAsync(int id);
        Task CreateAsync(EmployeeModel employee);
        Task UpdateAsync(int id, EmployeeModel employee);
        Task DeleteAsync(int id);
    }
}
