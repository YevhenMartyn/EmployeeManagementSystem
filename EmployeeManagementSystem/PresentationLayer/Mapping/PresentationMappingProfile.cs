using AutoMapper;

namespace PresentationLayer.Mapping
{
    public class PresentationMappingProfile : Profile
    {
        public PresentationMappingProfile()
        {
            //Departments
            CreateMap<BusinessLogicLayer.Models.DepartmentModel, Models.DepartmentDTO>().ReverseMap();

            //Employees
            CreateMap<BusinessLogicLayer.Models.EmployeeModel, Models.EmployeeDTO>().ReverseMap();
        }
    }
}
