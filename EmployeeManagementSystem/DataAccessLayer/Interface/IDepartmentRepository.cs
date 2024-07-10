using DataAccessLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IDepartmentRepository
    {
        List<Department> GetAll();
        Department GetById(int id);
        void Create(Department department);
        void Update(Department department);
        void Delete(int id);
        void SaveChanges();
    }
}
