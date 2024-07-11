using DataAccessLayer.Entities;

namespace DataAccessLayer.Interface
{
    public interface IDepartmentRepository
    {
        IList<DepartmentEntity> GetAll();
        DepartmentEntity GetById(int id);
        void Create(DepartmentEntity department);
        void Update(DepartmentEntity department);
        void Delete(int id);
    }
}
