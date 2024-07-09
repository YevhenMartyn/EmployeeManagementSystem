using AutoMapper;


namespace PresentationLayer.Mapping
{
    public class PresentationMappingProfile : Profile
    {
        public PresentationMappingProfile()
        {
            //Departments
            CreateMap<BusinessLogicLayer.Models.Department, Models.DepartmentDTO>().ReverseMap();

            //Employees
            CreateMap<BusinessLogicLayer.Models.Employee, Models.EmployeeDTO>().ReverseMap();
        }
    }
}
