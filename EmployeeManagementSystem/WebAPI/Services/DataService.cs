using WebAPI.Models;

namespace WebAPI.Services
{
    public static class DataService
    {
        static List<Employee> Employees { get; set; }
        private static int _nextEmployeeId = 4;

        static List<Department> Departments { get; set; }
        private static int _nextDepartmentId = 3;
        private static Department _defaultDepartment = new Department { Id = 0, Name = "No department" };

        static DataService()
        {
            Departments = new List<Department> { new Department {Id = 1, Name = "department1" },
                                                 new Department {Id = 2, Name = "department2" } };


            Employees = new List<Employee> { new Employee {Id = 1, Name = "employee1", Position = "position1", Department = GetDepartmentById(1), StartDate = new DateTime(2023, 4, 7) }, 
                                             new Employee {Id = 2, Name = "employee2", Position = "position2", Department = GetDepartmentById(2), StartDate = new DateTime(2023, 7, 15) },
                                             new Employee {Id = 3, Name = "employee3", Position = "position3", Department = GetDepartmentById(2), StartDate = new DateTime(2023, 9, 18) } };
        }

        //Employees ----------------

        public static List<Employee> GetAllEmployees() => Employees;
        public static Employee? GetEmployeeById(int id) => Employees.FirstOrDefault(employee => employee.Id == id);

        public static void AddEmployee(Employee employee) {

            // Check if the department exists
            var department = GetDepartmentById(employee.Department.Id);
            if (department == null || department.Name != employee.Department.Name)
            {
                throw new Exception("Invalid department.");
            }

            employee.Department = department;
            employee.Id = _nextEmployeeId;
            _nextEmployeeId++;
            Employees.Add(employee);
        }

        public static void DeleteEmployeeById(int id) {
            var employee = GetEmployeeById(id);
            if (employee is null)
                return;

            Employees.Remove(employee);
        }

        public static void UpdateEmployee(Employee employee) {

            // Check if the department exists
            var department = GetDepartmentById(employee.Department.Id);
            if (department == null || department.Name != employee.Department.Name)
            {
                throw new Exception("Invalid department.");
            }

            employee.Department = department;
            int index = Employees.FindIndex(e => e.Id == employee.Id);
            if (index == -1)
                return;

            Employees[index] = employee;
        }

        //Departments ------------------

        public static List<Department> GetAllDepartments() => Departments;
        public static Department? GetDepartmentById(int id) => Departments.FirstOrDefault(department => department.Id == id);

        public static void AddDepartment(Department department)
        {
            department.Id = _nextDepartmentId;
            _nextDepartmentId++;
            Departments.Add(department);
        }

        public static void DeleteDepartmentById(int id)
        {
            var department = GetDepartmentById(id);
            if (department is null)
                return;

            // Update employees in this department to the default department
            foreach (Employee employee in Employees.Where(e => e.Department?.Id == id))
            {
                employee.Department = _defaultDepartment;
            }

            Departments.Remove(department);
        }

        public static void UpdateDepartment(Department department)
        {
            int index = Departments.FindIndex(d => d.Id == department.Id);
            if (index == -1)
                return;

            Departments[index] = department;
        }
    }
}
