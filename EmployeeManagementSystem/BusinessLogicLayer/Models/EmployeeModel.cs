namespace BusinessLogicLayer.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
