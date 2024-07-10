using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public EmployeeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Create(EmployeeEntity entity)
        {
            _dbContext.Employees.Add(entity);
            SaveChanges();
        }

        public void Delete(int id)
        {
            _dbContext.Employees.Remove(GetById(id));
            SaveChanges();
        }

        public List<EmployeeEntity> GetAll(Expression<Func<EmployeeEntity, bool>> filter = null)
        {
            IQueryable<EmployeeEntity> query = _dbContext.Employees.AsNoTracking(); 

            if (filter != null)
            {
                query = query.Where(filter); 
            }

            return query.ToList();
        }

        public EmployeeEntity GetById(int id)
        {
            IQueryable<EmployeeEntity> query = _dbContext.Employees.AsNoTracking();

            return query.FirstOrDefault(e => e.Id == id);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        public void Update(EmployeeEntity entity)
        {
            if (entity.DepartmentId != null && !_dbContext.Departments.Any(d => d.Id == entity.DepartmentId))
            {
                throw new Exception("Invalid DepartmentId");
            }

            if (!entity.DepartmentId.HasValue)
            {
                entity.DepartmentId = -1;
            }

            _dbContext.Employees.Update(entity);
            SaveChanges();
        }
    }
}
