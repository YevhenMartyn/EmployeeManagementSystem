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
using System.Net;

namespace BusinessLogicLayer.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IValidator<DepartmentModel> _departmentValidator;
        private readonly ILogger<DepartmentService> _logger;
        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper,
            IEmployeeRepository employeeRepository,
            IValidator<DepartmentModel> departmentValidator,
            ILogger<DepartmentService> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _departmentValidator = departmentValidator;
        }

        public IList<DepartmentModel> GetAll()
        {
            _logger.LogInformation("Fetching all departments");
            IList<DepartmentModel> departments = _mapper.Map<IList<DepartmentModel>>(_repository.GetAll());
            return departments;
        }

        public DepartmentModel GetById(int id)
        {
            _logger.LogInformation($"Fetching department with ID {id}");
            DepartmentModel department = _mapper.Map<DepartmentModel>(_repository.GetById(id));

            if (department == null)
            {
                CustomException ex = new CustomException($"Department with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogWarning(ex.Message);
                throw ex;
            }
            return department;
        }

        public void Create(DepartmentModel department)
        {
            var validationResult = Validate(department);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                _logger.LogError(ex.Message);
                throw ex;
            }
            _logger.LogInformation($"Department with ID {department.Id} added successfully");
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
        }

        public void Update(int id, DepartmentModel department)
        {
            department.Id = id;
            var validationResult = Validate(department);
            if (!validationResult.IsValid)
            {
                CustomException ex = new CustomException($"Invalid model: {validationResult.ToString()}", StatusCodes.Status400BadRequest);
                _logger.LogError(ex.Message);
                throw ex;
            }

            DepartmentModel existingDepartment = _mapper.Map<DepartmentModel>(_repository.GetById(id));
            if (existingDepartment == null)
            {
                CustomException ex = new CustomException($"Department with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogError(ex.Message);
                throw ex;
            }

            _logger.LogInformation($"Department with ID {id} updated successfully");
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
        }

        public void Delete(int id)
        {
            var department = _mapper.Map<DepartmentModel>(_repository.GetById(id));

            if (department == null)
            {
                CustomException ex = new CustomException($"Department with ID {id} not found", StatusCodes.Status404NotFound);
                _logger.LogError(ex.Message);
                throw ex;
            }

            _logger.LogInformation($"Department with ID {id} deleted successfully");
            _repository.Delete(id);
        }

        private ValidationResult Validate(DepartmentModel department)
        {
            return _departmentValidator.Validate(department);
        }
    }
}
