using AutoMapper;

namespace BusinessLogicLayer.Mapping
{
    public class BusinessMappingProfile : Profile
    {
        public BusinessMappingProfile()
        {
            //Departments
            CreateMap<BusinessLogicLayer.Models.DepartmentModel, DataAccessLayer.Entities.DepartmentEntity>().ReverseMap();

            //Employees
            CreateMap<BusinessLogicLayer.Models.EmployeeModel, DataAccessLayer.Entities.EmployeeEntity>().ReverseMap();
        }
    }
}