using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class EmployeeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
