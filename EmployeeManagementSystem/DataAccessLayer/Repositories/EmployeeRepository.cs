using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;

namespace DataAccessLayer.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDistributedCache _cache;
        private readonly string cacheKeyPrefix = "Employee_";

        public EmployeeRepository(ApplicationDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public void Create(EmployeeEntity entity)
        {
            _dbContext.Employees.Add(entity);
            SaveChanges();
            InvalidateCache(entity.Id);
        }

        public void Delete(int id)
        {
            _dbContext.Employees.Remove(GetById(id));
            SaveChanges();
            InvalidateCache(id);
        }

        public IList<EmployeeEntity> GetAll(Expression<Func<EmployeeEntity, bool>> filter = null)
        {
            string cacheKey = $"{cacheKeyPrefix}All";
            IList<EmployeeEntity> employees = GetCache<List<EmployeeEntity>>(cacheKey);

            if (employees == null)
            {
                IQueryable<EmployeeEntity> query = _dbContext.Employees.AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                employees = query.ToList();
                SetCache(cacheKey, employees);
            }

            return employees;
        }

        public EmployeeEntity GetById(int id)
        {
            string cacheKey = $"{cacheKeyPrefix}{id}";
            EmployeeEntity employee = GetCache<EmployeeEntity>(cacheKey);

            if (employee == null)
            {
                employee = _dbContext.Employees.AsNoTracking().FirstOrDefault(e => e.Id == id);
                if (employee != null)
                {
                    SetCache(cacheKey, employee);
                }
            }

            return employee;
        }

        public void Update(EmployeeEntity entity)
        {
            if (entity.DepartmentId != null && !_dbContext.Departments.Any(d => d.Id == entity.DepartmentId))
            {
                throw new Exception("Invalid DepartmentId");
            }

            _dbContext.Employees.Update(entity);
            SaveChanges();
            InvalidateCache(entity.Id);
        }

        private void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private void InvalidateCache(int id)
        {
            _cache.Remove($"{cacheKeyPrefix}{id}");
            _cache.Remove($"{cacheKeyPrefix}All");
        }

        private T GetCache<T>(string cacheKey)
        {
            var cachedData = _cache.GetString(cacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        private void SetCache<T>(string cacheKey, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // Cache expiration time
            };
            _cache.SetString(cacheKey, JsonSerializer.Serialize(data), options);
        }
    }
}
