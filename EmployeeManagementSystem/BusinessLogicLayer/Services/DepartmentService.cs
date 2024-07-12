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
        public DepartmentService(IGenericRepository<DepartmentEntity> repository,
                                 IMapper mapper,
                                 IValidator<DepartmentModel> departmentValidator,
                                 ILogger<DepartmentService> logger)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
            _departmentValidator = departmentValidator;
        }

        public async Task<IList<DepartmentModel>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all departments");
            IList<DepartmentModel> departments = _mapper.Map<IList<DepartmentModel>>(await _repository.GetAllAsync());
            return departments;
        }

        public async Task<DepartmentModel> GetByIdAsync(int id)
        {
            _logger.LogInformation($"Fetching department with ID {id}");
            DepartmentModel department = _mapper.Map<DepartmentModel>(await _repository.GetByIdAsync(id));

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
            await _repository.CreateAsync(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
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
            await _repository.UpdateAsync(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
        }

        public async Task DeleteAsync(int id)
        {
            var existingDepartment = await GetByIdAsync(id);
            _logger.LogInformation($"Department with ID {id} deleted successfully");
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
