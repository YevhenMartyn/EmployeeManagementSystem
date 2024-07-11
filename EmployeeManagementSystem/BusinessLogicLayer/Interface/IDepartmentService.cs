using BusinessLogicLayer.Models;

namespace BusinessLogicLayer.Interface
{
    public interface IDepartmentService
    {
        IList<DepartmentModel> GetAll();
        DepartmentModel GetById(int id);
        void Create(DepartmentModel department);
        void Update(int id, DepartmentModel department);
        void Delete(int id);
    }
}
