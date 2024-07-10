using AutoMapper;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        public EmployeeService(IEmployeeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public void Create(Employee employee)
        {
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.Employee>(employee));
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public List<Employee> GetAll()
        {
            List<Employee> employees = _mapper.Map<List<Employee>>(_repository.GetAll());
            return employees;

        }

        public Employee GetById(int id)
        {
            Employee employee = _mapper.Map<Employee>(_repository.GetById(id));
            return employee;
        }

        public void Update(Employee employee)
        {
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.Employee>(employee));
        }
    }
}
