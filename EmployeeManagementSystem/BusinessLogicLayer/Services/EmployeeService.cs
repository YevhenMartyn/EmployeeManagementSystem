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
        public void Create(EmployeeModel employee)
        {
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
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

        public void Update(EmployeeModel employee)
        {
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.EmployeeEntity>(employee));
        }
    }
}
