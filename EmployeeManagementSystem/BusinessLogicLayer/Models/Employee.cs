
namespace BusinessLogicLayer.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public int DepartmentId { get; set; }
        public DateTime StartDate { get; set; }
    }
}
