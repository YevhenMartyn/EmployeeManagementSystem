namespace BusinessLogicLayer.Models
{
    public class EmployeeFilterModel
    {
        public string? Name { get; set; }
        public string? Position { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
