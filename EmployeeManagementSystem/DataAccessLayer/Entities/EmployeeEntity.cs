using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public class EmployeeEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        public DepartmentEntity? Department { get; set; }
        public DateTime StartDate { get; set; }
    }
}
