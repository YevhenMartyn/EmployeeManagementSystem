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

        public void Create(DepartmentEntity entity)
        {
            _dbContext.Departments.Add(entity);
            SaveChanges();
            InvalidateCache(entity.Id);
        }

        public void Delete(int id)
        {
            _dbContext.Departments.Remove(GetById(id));
            SaveChanges();
            InvalidateCache(id);
        }

        public DepartmentEntity GetById(int id)
        {
            string cacheKey = $"{cacheKeyPrefix}{id}";
            DepartmentEntity department = GetCache<DepartmentEntity>(cacheKey);

            if (department == null)
            {
                department = _dbContext.Departments.AsNoTracking().FirstOrDefault(d => d.Id == id);
                if (department != null)
                {
                    SetCache(cacheKey, department);
                }
            }

            return department;
        }

        public IList<DepartmentEntity> GetAll()
        {
            string cacheKey = $"{cacheKeyPrefix}All";
            IList<DepartmentEntity> departments = GetCache<List<DepartmentEntity>>(cacheKey);

            if (departments == null)
            {
                IQueryable<DepartmentEntity> query = _dbContext.Departments.AsNoTracking();
                departments = query.ToList();
                SetCache(cacheKey, departments);
            }

            return departments;
        }

        public void Update(DepartmentEntity entity)
        {
            _dbContext.Departments.Update(entity);
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
