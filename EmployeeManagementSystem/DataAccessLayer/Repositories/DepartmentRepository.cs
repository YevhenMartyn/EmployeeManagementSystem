using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public DepartmentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create(Department entity)
        {
            _dbContext.Departments.Add(entity);
            SaveChanges();
        }

        public void Delete(int id)
        {
            _dbContext.Departments.Remove(GetById(id));
            SaveChanges();
        }

        public Department GetById(int id)
        {
            IQueryable<Department> query = _dbContext.Departments.AsNoTracking(); 
            return query.FirstOrDefault(d => d.Id == id);
        }

        public List<Department> GetAll()
        {
            IQueryable<Department> query = _dbContext.Departments.AsNoTracking();
            return query.ToList();
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Department entity)
        {
            _dbContext.Departments.Update(entity);
            SaveChanges();
        }
    }
}
