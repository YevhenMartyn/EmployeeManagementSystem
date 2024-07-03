using WebAPI.Models;

namespace WebAPI.Services
{
    public static class DataService
    {
        static List<Employee> Employees { get; set; }
        static int nextEmployeeId = 3;

        static DataService()
        {
            Employees = new List<Employee> { new Employee {Name = "employee1" }, 
                                             new Employee {Name = "employee2" },
                                             new Employee {Name = "employee3" } };
        }

        //Employees
        public static List<Employee> GetAllEmployees(int id) => Employees;
        public static Employee GetEmployeeById(int id) => Employees.FirstOrDefault(employee => employee.Id == id);

        public static void AddEmployee(Employee employee) {
            employee.Id = nextEmployeeId;
            nextEmployeeId++;
            Employees.Add(employee);
        }

        public static void DeleteEmployeeById(int id) {
            var employee = GetEmployeeById(id);
            if (employee is null)
                return;

            Employees.Remove(employee);
        }

        public static void UpdateEmployee(Employee employee) {
            int index = Employees.FindIndex(e => e.Id == employee.Id);
            if (index == -1)
                return;

            Employees[index] = employee;
        }

    }
}
