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

        public async Task CreateAsync(EmployeeEntity entity)
        {
            await _dbContext.Employees.AddAsync(entity);
            await SaveChangesAsync();
            await InvalidateCacheAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbContext.Employees.Remove(entity);
                await SaveChangesAsync();
                await InvalidateCacheAsync(id);
            }
        }

        public async Task<IList<EmployeeEntity>> GetAllAsync(Expression<Func<EmployeeEntity, bool>> filter = null)
        {
            string cacheKey = $"{cacheKeyPrefix}All";
            IList<EmployeeEntity> employees = await GetCacheAsync<List<EmployeeEntity>>(cacheKey);

            if (employees == null)
            {
                IQueryable<EmployeeEntity> query = _dbContext.Employees.AsNoTracking();

                if (filter != null)
                {
                    query = query.Where(filter);
                }

                employees = await query.ToListAsync();
                await SetCacheAsync(cacheKey, employees);
            }

            return employees;
        }

        public async Task<EmployeeEntity> GetByIdAsync(int id)
        {
            string cacheKey = $"{cacheKeyPrefix}{id}";
            EmployeeEntity employee = await GetCacheAsync<EmployeeEntity>(cacheKey);

            if (employee == null)
            {
                employee = await _dbContext.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                if (employee != null)
                {
                    await SetCacheAsync(cacheKey, employee);
                }
            }

            return employee;
        }

        public async Task UpdateAsync(EmployeeEntity entity)
        {
            if (entity.DepartmentId != null && !await _dbContext.Departments.AnyAsync(d => d.Id == entity.DepartmentId))
            {
                throw new Exception("Invalid DepartmentId");
            }

            _dbContext.Employees.Update(entity);
            await SaveChangesAsync();
            await InvalidateCacheAsync(entity.Id);
        }

        private async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        private async Task InvalidateCacheAsync(int id)
        {
            await _cache.RemoveAsync($"{cacheKeyPrefix}{id}");
            await _cache.RemoveAsync($"{cacheKeyPrefix}All");
        }

        private async Task<T> GetCacheAsync<T>(string cacheKey)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedData))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        private async Task SetCacheAsync<T>(string cacheKey, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // Cache expiration time
            };
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(data), options);
        }
    }
}
