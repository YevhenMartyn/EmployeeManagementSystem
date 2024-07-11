using AutoMapper;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using DataAccessLayer.Interface;
using FluentValidation;
using FluentValidation.Results;

namespace BusinessLogicLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly IValidator<EmployeeModel> _employeeValidator;
        public EmployeeService(IEmployeeRepository repository, IMapper mapper, IValidator<EmployeeModel> employeeValidator)
        {
            _repository = repository;
            _mapper = mapper;
            _employeeValidator = employeeValidator;
        }
        public IList<EmployeeModel> GetAll()
        {
            IList<EmployeeModel> employees = _mapper.Map<IList<EmployeeModel>>(_repository.GetAll());
            return employees;
        }

        public EmployeeModel GetById(int id)
        {
            EmployeeModel employee = _mapper.Map<EmployeeModel>(_repository.GetById(id));
            return employee;
        }

        public void Create(EmployeeModel employee)
        {
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
        }

        public void Update(EmployeeModel employee)
        {
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        private ValidationResult Validate(EmployeeModel employee)
        {
            return _employeeValidator.Validate(employee);
        }
    }
}
