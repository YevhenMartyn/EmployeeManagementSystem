using AutoMapper;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Validators;
using DataAccessLayer.Interface;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<EmployeeModel> _employeeValidator;
        private readonly ILogger<EmployeeService> _logger;
        public EmployeeService(IEmployeeRepository repository,
                               IMapper mapper,
                               IValidator<EmployeeModel> employeeValidator,
                               ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _employeeValidator = employeeValidator;
            _logger = logger;
        }

        public async Task<IList<EmployeeModel>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all employees");
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(await _repository.GetAllAsync());
            return employees;
        }

        public async Task<IList<EmployeeModel>> GetAllAsync(int? departmentId, DateTime? fromDate, DateTime? toDate)
        {
            IEnumerable<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(await _repository.GetAllAsync());

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
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(await _repository.GetAllAsync());
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
                CustomException ex = new CustomException("No employee found", StatusCodes.Status404NotFound);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            return employees;
        }

            public async Task<EmployeeModel> GetByIdAsync(int id)
            {
                _logger.LogInformation($"Fetching employee with ID {id}");
                EmployeeModel employee = _mapper.Map<EmployeeModel>(await _repository.GetByIdAsync(id));

                if (employee == null)
                {
                    CustomException ex = new CustomException($"Employee with ID {id} not found", StatusCodes.Status404NotFound);
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
                    CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                    _logger.LogError(ex.Message);
                    throw ex;
                }
                _logger.LogInformation($"Employee with ID {employee.Id} added successfully");
                await _repository.CreateAsync(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
            }

            public async Task UpdateAsync(int id, EmployeeModel employee)
            {
                employee.Id = id;
                var validationResult = await ValidateAsync(employee);
                if (!validationResult.IsValid)
                {
                    CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                    _logger.LogError(ex.Message);
                    throw ex;
                }

                var existingEmployee = await GetByIdAsync(id);
                _logger.LogInformation($"Employee with ID {id} updated successfully");
                await _repository.UpdateAsync(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
            }

            public async Task DeleteAsync(int id)
            {
                var existingEmployee = await GetByIdAsync(id);
                _logger.LogInformation($"Employee with ID {id} deleted successfully");
                await _repository.DeleteAsync(id);
            }

            private async Task<ValidationResult> ValidateAsync(EmployeeModel employee)
            {
                var validator = new EmployeeValidator();
                var validationResult = await validator.ValidateAsync(employee);
                return validationResult;
            }
        }
    }
