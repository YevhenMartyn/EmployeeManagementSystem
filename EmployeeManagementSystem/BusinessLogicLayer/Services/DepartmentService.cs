using AutoMapper;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using DataAccessLayer.Interface;

namespace BusinessLogicLayer.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        public DepartmentService(IDepartmentRepository repository, IMapper mapper, IEmployeeRepository employeeRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _employeeRepository = employeeRepository;
        }
        public void Create(Department department)
        {
            _repository.Create(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
        }

        public void Delete(int id)
        {
            var employees = _employeeRepository.GetAll(e => e.DepartmentId == id);
            foreach (var employee in employees)
            {
                employee.DepartmentId = -1;
                _employeeRepository.Update(employee);
            }
            _repository.Delete(id);
        }

        public IList<Department> GetAll()
        {
            IList<Department> departments = _mapper.Map<IList<Department>>(_repository.GetAll());
            return departments;

        }

        public Department GetById(int id)
        {
            Department department = _mapper.Map<Department>(_repository.GetById(id));    
            return department;
        }

        public void Update(Department department)
        {
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
        }
    }
}
