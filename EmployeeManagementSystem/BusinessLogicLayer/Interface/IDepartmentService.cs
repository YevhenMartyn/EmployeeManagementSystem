using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IDepartmentService
    {
        List<Department> GetAll();
        Department GetById(int id);
        void Create(Department department);
        void Update(Department department);
        void Delete(int id);
    }
}
