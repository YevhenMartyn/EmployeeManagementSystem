using WebAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebAPI.Services
{
    public static class DataService
    {
        static Dictionary<int, Employee> Employees { get; set; }
        private static int _nextEmployeeId = 4;

        static Dictionary<int, Department> Departments { get; set; }
        private static int _nextDepartmentId = 3;
        private static Department _defaultDepartment = new Department { Id = 0, Name = "No department" };

        static DataService()
        {
            Departments = new Dictionary<int, Department>
            {
                { 1, new Department {Id = 1, Name = "department1" } },
                { 2, new Department {Id = 2, Name = "department2" } }
            };

            Employees = new Dictionary<int, Employee>
            {
                { 1, new Employee {Id = 1, Name = "employee1", Position = "position1", Department = GetDepartmentById(1), StartDate = new DateTime(2023, 4, 7) } },
                { 2, new Employee {Id = 2, Name = "employee2", Position = "position2", Department = GetDepartmentById(2), StartDate = new DateTime(2023, 7, 15) } },
                { 3, new Employee {Id = 3, Name = "employee3", Position = "position3", Department = GetDepartmentById(2), StartDate = new DateTime(2023, 9, 18) } }
            };
        }

        // Employees ----------------

        public static List<Employee> GetAllEmployees() => Employees.Values.ToList();
        public static Employee? GetEmployeeById(int id) => Employees.TryGetValue(id, out var employee) ? employee : null;

        public static void AddEmployee(Employee employee)
        {
            // Check if the department exists
            var department = GetDepartmentById(employee.Department.Id);
            if (department == null || department.Name != employee.Department.Name)
            {
                throw new Exception("Invalid department.");
            }

            employee.Department = department;
            employee.Id = _nextEmployeeId;
            _nextEmployeeId++;
            Employees[employee.Id] = employee;
        }

        public static void DeleteEmployeeById(int id)
        {
            if (Employees.ContainsKey(id))
            {
                Employees.Remove(id);
            }
        }

        public static void UpdateEmployee(Employee employee)
        {
            if (employee.Department != null)
            {
                // Check if the department exists
                var department = GetDepartmentById(employee.Department.Id);
                if (department == null || department.Name != employee.Department.Name)
                {
                    throw new Exception("Invalid department.");
                }

                employee.Department = department;
            }
            else
            {
                employee.Department = _defaultDepartment;
            }

            Employees[employee.Id] = employee;
        }

        // Departments ------------------

        public static List<Department> GetAllDepartments() => Departments.Values.ToList();
        public static Department? GetDepartmentById(int id) => Departments.TryGetValue(id, out var department) ? department : null;

        public static void AddDepartment(Department department)
        {
            department.Id = _nextDepartmentId;
            _nextDepartmentId++;
            Departments[department.Id] = department;
        }

        public static void DeleteDepartmentById(int id)
        {
            if (Departments.ContainsKey(id))
            {
                // Update employees in this department to the default department
                foreach (var employee in Employees.Values.Where(e => e.Department?.Id == id))
                {
                    employee.Department = _defaultDepartment;
                }

                Departments.Remove(id);
            }
        }

        public static void UpdateDepartment(Department department)
        {
            Departments[department.Id] = department;
        }
    }
}
