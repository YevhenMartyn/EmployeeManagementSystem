using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [MaxLength(30)]
        public string Position { get; set; }
        public Department Department { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }
}
