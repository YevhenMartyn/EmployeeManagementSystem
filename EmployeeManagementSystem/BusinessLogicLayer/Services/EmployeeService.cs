using AutoMapper;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using DataAccessLayer.Interface;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BusinessLogicLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<EmployeeModel> _employeeValidator;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IDepartmentService _departmentService;
        public EmployeeService(IEmployeeRepository repository,
                               IMapper mapper,
                               IValidator<EmployeeModel> employeeValidator,
                               ILogger<EmployeeService> logger,
                               IDepartmentService departmentService)
        {
            _repository = repository;
            _mapper = mapper;
            _employeeValidator = employeeValidator;
            _logger = logger;
            _departmentService = departmentService;
        }

        public IList<EmployeeModel> GetAll()
        {
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(_repository.GetAll());
            return employees;
        }

        public IList<EmployeeModel> GetAll(int? departmentId, DateTime? fromDate, DateTime? toDate)
        {
            IEnumerable<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(_repository.GetAll());

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

        public IList<EmployeeModel> GetAll(string? name, string? position, int? departmentId, DateTime? startDate)
        {
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(_repository.GetAll());
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

        public EmployeeModel GetById(int id)
        {
            EmployeeModel employee = _mapper.Map<EmployeeModel>(_repository.GetById(id));
            if (employee == null)
            {
                CustomException ex = new CustomException($"Employee with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            return employee;
        }

        public void Create(EmployeeModel employee)
        {
            var validationResult = Validate(employee);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                _logger.LogWarning(ex.Message);
                throw ex;
            }
            CheckIfDepartmentExists((int)employee.DepartmentId);

            _logger.LogInformation($"Employee with ID {employee.Id} added successfully");
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
        }

        public void Update(int id, EmployeeModel employee)
        {
            employee.Id = id;
            var validationResult = Validate(employee);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                _logger.LogWarning(ex.Message);
                throw ex;
            }
            if (employee.DepartmentId != null)
            {
                CheckIfDepartmentExists((int)employee.DepartmentId);
            }
            EmployeeModel existingEmployee = _mapper.Map<EmployeeModel>(_repository.GetById(id));

            if (existingEmployee == null)
            {
                CustomException ex = new CustomException($"Employee with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            _logger.LogInformation($"Employee with ID {id} updated successfully");
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
        }

        public void Delete(int id)
        {
            var employee = _mapper.Map<EmployeeModel>(_repository.GetById(id));

            if (employee == null)
            {
                CustomException ex = new CustomException($"Employee with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogWarning(ex.Message);
                throw ex;
            }

            _logger.LogInformation($"Employee with ID {id} deleted successfully");
            _repository.Delete(id);
        }

        private void CheckIfDepartmentExists(int id) {
            try
            {
                _departmentService.GetById(id);
            }
            catch (CustomException ex)
            {
                throw ex;
            }
        }

        private ValidationResult Validate(EmployeeModel employee)
        {
            return _employeeValidator.Validate(employee);
        }
    }
}
