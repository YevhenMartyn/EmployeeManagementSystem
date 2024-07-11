using System.ComponentModel.DataAnnotations;

namespace PresentationLayer
{
    public class AppSettings
    {
        [Required]
        public string ApplicationName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MaxEmployees must be greater than 0.")]
        public int MaxEmployees { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "MinDepartmentId must be greater than 0.")]
        public int MinDepartmentId { get; set; }
    }
}