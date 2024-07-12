using DataAccessLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IDepartmentRepository
    {
        Task<IList<DepartmentEntity>> GetAllAsync();
        Task<DepartmentEntity> GetByIdAsync(int id);
        Task CreateAsync(DepartmentEntity department);
        Task UpdateAsync(DepartmentEntity department);
        Task DeleteAsync(int id);
    }
}
