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

        public async Task<IList<EmployeeModel>> GetAllAsync(EmployeeFilterModel? filter)
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefix()}All";
            IEnumerable<EmployeeEntity> employeesEntity = await _cacheService.GetCacheAsync<IEnumerable<EmployeeEntity>>(cacheKey);
            if (employeesEntity == null)
            {
                employeesEntity = await _repository.GetAllAsync();
                await _cacheService.SetCacheAsync(cacheKey, employeesEntity);
            }

            IEnumerable<EmployeeModel> employees = _mapper.Map<IEnumerable<EmployeeModel>>(employeesEntity);

            if (!string.IsNullOrEmpty(filter.Name))
            {
                employees = employees.Where(e => e.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filter.Position))
            {
                employees = employees.Where(e => e.Position.Contains(filter.Position, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.DepartmentId != null)
            {
                employees = employees.Where(n => n.DepartmentId == filter.DepartmentId);
            }

            if (filter.FromDate.HasValue)
            {
                employees = employees.Where(e => e.StartDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                employees = employees.Where(e => e.StartDate <= filter.ToDate.Value);
            }

            if (!(filter.StartDate == null || filter.StartDate == new DateTime(1, 1, 1))) //default date
            {
                employees = employees.Where(e => e.StartDate.Date == filter.StartDate);
            }

            if (employees.Count() == 0)
            {
                CustomException ex = new CustomException("No employee found", 404);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            return employees.ToList();

        }

        public async Task<EmployeeModel> GetByIdAsync(int id)
        {
            string cacheKey = $"{_cacheService.GetCacheKeyPrefix()}{id}";

            EmployeeEntity employeeEntity = await _cacheService.GetCacheAsync<EmployeeEntity>(cacheKey);
            if (employeeEntity == null)
            {
                employeeEntity = await _repository.GetByIdAsync(id);
                await _cacheService.SetCacheAsync(cacheKey, employeeEntity);
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
