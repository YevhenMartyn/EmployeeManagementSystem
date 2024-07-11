
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Models
{
    public class DepartmentModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
