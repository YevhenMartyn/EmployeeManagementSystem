using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace DataAccessLayer.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDistributedCache _cache;
        private readonly string cacheKeyPrefix = "Department_";

        public DepartmentRepository(ApplicationDbContext dbContext, IDistributedCache cache)
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        public async Task<IList<DepartmentEntity>> GetAllAsync()
        {
            string cacheKey = $"{cacheKeyPrefix}All";
            IList<DepartmentEntity> departments = await GetCacheAsync<List<DepartmentEntity>>(cacheKey);

            if (departments == null)
            {
                IQueryable<DepartmentEntity> query = _dbContext.Departments.AsNoTracking();
                departments = await query.ToListAsync();
                await SetCacheAsync(cacheKey, departments);
            }

            return departments;
        }

        public async Task<DepartmentEntity> GetByIdAsync(int id)
        {
            string cacheKey = $"{cacheKeyPrefix}{id}";
            DepartmentEntity department = await GetCacheAsync<DepartmentEntity>(cacheKey);

            if (department == null)
            {
                department = await _dbContext.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
                if (department != null)
                {
                    await SetCacheAsync(cacheKey, department);
                }
            }

            return department;
        }

        public async Task CreateAsync(DepartmentEntity entity)
        {
            await _dbContext.Departments.AddAsync(entity);
            await SaveChangesAsync();
            await InvalidateCacheAsync(entity.Id);
        }

        public async Task UpdateAsync(DepartmentEntity entity)
        {
            _dbContext.Departments.Update(entity);
            await SaveChangesAsync();
            await InvalidateCacheAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbContext.Departments.Remove(entity);
                await SaveChangesAsync();
                await InvalidateCacheAsync(id);
            }
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
