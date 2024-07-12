using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IDepartmentService
    {
        Task<IList<DepartmentModel>> GetAllAsync();
        Task<DepartmentModel> GetByIdAsync(int id);
        Task CreateAsync(DepartmentModel department);
        Task UpdateAsync(int id, DepartmentModel department);
        Task DeleteAsync(int id);
    }
}
