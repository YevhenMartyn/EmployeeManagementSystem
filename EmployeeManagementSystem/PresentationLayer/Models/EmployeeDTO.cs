using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class EmployeeDTO
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
