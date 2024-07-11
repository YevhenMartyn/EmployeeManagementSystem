using System.ComponentModel.DataAnnotations;

namespace PresentationLayer.Models
{
    public class DepartmentDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
