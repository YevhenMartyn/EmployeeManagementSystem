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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        //private readonly IEmployeeRepository _repository;
        private readonly IGenericRepository<EmployeeEntity> _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<EmployeeModel> _employeeValidator;
        private readonly ILogger<EmployeeService> _logger;
        private readonly ICacheService<EmployeeEntity> _cacheService;
        public EmployeeService(IGenericRepository<EmployeeEntity> repository,
                               IMapper mapper,
                               IValidator<EmployeeModel> employeeValidator,
                               ILogger<EmployeeService> logger,
                               ICacheService<EmployeeEntity> cacheService)
        {
            _repository = repository;
            _mapper = mapper;
            _employeeValidator = employeeValidator;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<IList<EmployeeModel>> GetAllAsync()
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefix()}All";
            IList<EmployeeEntity> employeesEntity = await _cacheService.GetCacheAsync<IList<EmployeeEntity>>(cacheKey);
            if (employeesEntity == null)
            {
                employeesEntity = await _repository.GetAllAsync();
            }
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(employeesEntity);
            await _cacheService.SetCacheAsync(cacheKey, employeesEntity);
            return employees;
        }

        public async Task<IList<EmployeeModel>> GetAllAsync(int? departmentId, DateTime? fromDate, DateTime? toDate)
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefix()}All";
            IEnumerable<EmployeeEntity> employeesEntity = await _cacheService.GetCacheAsync<IEnumerable<EmployeeEntity>>(cacheKey);
            if (employeesEntity == null)
            {
                employeesEntity = await _repository.GetAllAsync();
            }
            IEnumerable<EmployeeModel> employees = _mapper.Map<IEnumerable<EmployeeModel>>(employeesEntity);

            if (departmentId != null)
            {
                employees = employees.Where(n => n.DepartmentId == departmentId);
            }

            if (fromDate.HasValue)
            {
                employees = employees.Where(e => e.StartDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                employees = employees.Where(e => e.StartDate <= toDate.Value);
            }

            return employees.ToList();
        }

        public async Task<IList<EmployeeModel>> GetAllAsync(string? name, string? position, int? departmentId, DateTime? startDate)
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefix()}All";
            IList<EmployeeEntity> employeesEntity = await _cacheService.GetCacheAsync<IList<EmployeeEntity>>(cacheKey);
            if (employeesEntity == null)
            {
                employeesEntity = await _repository.GetAllAsync();
            }
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(employeesEntity);

            if (!string.IsNullOrEmpty(name))
            {
                employees = employees.Where(e => e.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(position))
            {
                employees = employees.Where(e => e.Position.Contains(position, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (departmentId > 0)
            {
                employees = employees.Where(e => e.DepartmentId == departmentId).ToList();
            }

            if (startDate != new DateTime(1, 1, 1)) //default date
            {
                employees = employees.Where(e => e.StartDate.Date == startDate).ToList();
            }

            if (employees.Count == 0)
            {
                CustomException ex = new CustomException("No employee found", 404);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            return employees;
        }

        public async Task<EmployeeModel> GetByIdAsync(int id)
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefix()}{id}";

            EmployeeEntity employeeEntity = await _cacheService.GetCacheAsync<EmployeeEntity>(cacheKey);
            if (employeeEntity == null)
            {
                employeeEntity = await _repository.GetByIdAsync(id);
            }

            EmployeeModel employee = _mapper.Map<EmployeeModel>(employeeEntity);

            if (employee == null)
            {
                CustomException ex = new CustomException($"Employee with ID {id} not found", 404);
                _logger.LogWarning(ex.Message);
                throw ex;
            }
            return employee;
        }

        public async Task CreateAsync(EmployeeModel employee)
        {
            var validationResult = await ValidateAsync(employee);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", 400);
                _logger.LogError(ex.Message);
                throw ex;
            }

            _logger.LogInformation($"Employee with ID {employee.Id} added successfully");
            EmployeeEntity employeeEntity = _mapper.Map<EmployeeEntity>(employee);
            await _cacheService.InvalidateCacheAsync(employeeEntity.Id);
            await _repository.CreateAsync(employeeEntity);
        }

        public async Task UpdateAsync(int id, EmployeeModel employee)
        {
            employee.Id = id;
            var validationResult = await ValidateAsync(employee);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", 400);
                _logger.LogError(ex.Message);
                throw ex;
            }

            var existingEmployee = await GetByIdAsync(id);
            _logger.LogInformation($"Employee with ID {id} updated successfully");
            EmployeeEntity employeeEntity = _mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee);
            await _cacheService.InvalidateCacheAsync(id);
            await _repository.UpdateAsync(employeeEntity);
        }

        public async Task DeleteAsync(int id)
        {
            var existingEmployee = await GetByIdAsync(id);
            _logger.LogInformation($"Employee with ID {id} deleted successfully");
            await _cacheService.InvalidateCacheAsync(id);
            await _repository.DeleteAsync(id);
        }

        private async Task<ValidationResult> ValidateAsync(EmployeeModel employee)
        {
            var validationResult = await _employeeValidator.ValidateAsync(employee);
            return validationResult;
        }
    }
}
