using AutoMapper;

namespace BusinessLogicLayer.Mapping
{
    public class BusinessMappingProfile : Profile
    {
        public BusinessMappingProfile()
        {
            //Departments
            CreateMap<BusinessLogicLayer.Models.Department, DataAccessLayer.Entities.Department>().ReverseMap();

            //Employees
            CreateMap<BusinessLogicLayer.Models.Employee, DataAccessLayer.Entities.Employee>().ReverseMap();
        }
    }
}