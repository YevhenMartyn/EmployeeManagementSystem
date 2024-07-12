using AutoMapper;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Validators;
using DataAccessLayer.Entities;
using DataAccessLayer.Interface;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace BusinessLogicLayer.Services
{
    public class DepartmentService : IDepartmentService
    {
        //private readonly IDepartmentRepository _repository;
        private readonly IGenericRepository<DepartmentEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<DepartmentModel> _departmentValidator;
        private readonly ILogger<DepartmentService> _logger;
        private readonly ICacheService<DepartmentEntity> _cacheService;
        public DepartmentService(IGenericRepository<DepartmentEntity> repository,
                                 IMapper mapper,
                                 IValidator<DepartmentModel> departmentValidator,
                                 ILogger<DepartmentService> logger,
                                 ICacheService<DepartmentEntity> cacheService)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _departmentValidator = departmentValidator;
            _cacheService = cacheService;
        }

        public async Task<IList<DepartmentModel>> GetAllAsync()
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefixAsync()}All";
            IList<DepartmentEntity> departmentsEntity = await _cacheService.GetCacheAsync<IList<DepartmentEntity>>(cacheKey);
            if (departmentsEntity == null)
            {
                departmentsEntity = await _repository.GetAllAsync();
            }

            await _cacheService.SetCacheAsync(cacheKey, departmentsEntity);
            return _mapper.Map<IList<DepartmentModel>>(departmentsEntity);
        }

        public async Task<DepartmentModel> GetByIdAsync(int id)
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefixAsync()}{id}";

            DepartmentEntity departmentEntity = await _cacheService.GetCacheAsync<DepartmentEntity>(cacheKey);
            if (departmentEntity == null)
            {  
                departmentEntity = await _repository.GetByIdAsync(id); 
            }

            DepartmentModel department = _mapper.Map<DepartmentModel>(departmentEntity);

            if (department == null)
            {
                CustomException ex = new CustomException($"Department with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            return department;
        }

        public async Task CreateAsync(DepartmentModel department)
        {
            var validationResult = Validate(department);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                _logger.LogError(ex.Message);
                throw ex;
            }
            _logger.LogInformation($"Department with ID {department.Id} added successfully");
            DepartmentEntity departmentEntity = _mapper.Map<DepartmentEntity>(department);
            await _cacheService.InvalidateCacheAsync(departmentEntity.Id);
            await _repository.CreateAsync(departmentEntity);
        }

        public async Task UpdateAsync(int id, DepartmentModel department)
        {
            department.Id = id;
            var validationResult = Validate(department);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                _logger.LogError(ex.Message);
                throw ex;
            }

            var existingDepartment = await GetByIdAsync(id);
            _logger.LogInformation($"Department with ID {id} updated successfully");

            DepartmentEntity departmentEntity = _mapper.Map<DepartmentEntity>(department);
            await _cacheService.InvalidateCacheAsync(id);
            await _repository.UpdateAsync(departmentEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingDepartment = await GetByIdAsync(id);
            _logger.LogInformation($"Department with ID {id} deleted successfully");
            await _cacheService.InvalidateCacheAsync(id);
            await _repository.DeleteAsync(id);
        }

        private ValidationResult Validate(DepartmentModel department)
        {
            var validator = new DepartmentValidator();
            var validationResult = validator.Validate(department);
            return validationResult;
        }
    }
}
