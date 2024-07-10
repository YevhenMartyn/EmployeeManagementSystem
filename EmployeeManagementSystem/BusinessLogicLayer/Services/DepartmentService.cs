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
        public void Create(DepartmentModel department)
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

        public IList<DepartmentModel> GetAll()
        {
            IList<DepartmentModel> departments = _mapper.Map<IList<DepartmentModel>>(_repository.GetAll());
            return departments;

        }

        public DepartmentModel GetById(int id)
        {
            DepartmentModel department = _mapper.Map<DepartmentModel>(_repository.GetById(id));    
            return department;
        }

        public void Update(DepartmentModel department)
        {
            _repository.Update(_mapper.Map<DataAccessLayer.Entities.DepartmentEntity>(department));
        }
    }
}
