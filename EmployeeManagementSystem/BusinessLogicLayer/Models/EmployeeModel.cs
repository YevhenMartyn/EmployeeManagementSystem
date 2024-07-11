
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MaxLength(30)]
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }
}
