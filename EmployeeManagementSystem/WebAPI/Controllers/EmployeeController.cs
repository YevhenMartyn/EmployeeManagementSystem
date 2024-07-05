using Microsoft.AspNetCore.Mvc;
using WebAPI.ModelBinders;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        // GET all action
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllEmployees([FromQuery(Name = "filterByDepartmentName")] string? departmentName,
                                             [FromQuery(Name = "startedAfterDate")] DateTime? fromDate,
                                             [FromQuery(Name = "startedBeforeDate")] DateTime? toDate)
        {
            _logger.LogInformation("Fetching all employees");
            IEnumerable<Employee> employees = Services.DataService.GetAllEmployees();

            if (departmentName != null)
            {
                employees = Services.DataService.GetAllEmployees().Where(n => n.Department.Name == departmentName);
            }

            if (fromDate.HasValue)
            {
                employees = employees.Where(e => e.StartDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                employees = employees.Where(e => e.StartDate <= toDate.Value);
            }

            return Ok(employees);
        }

        // GET by Id action
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEmployeeById(int id)
        {
            _logger.LogInformation($"Fetching employee with ID {id}");
            var employee = Services.DataService.GetEmployeeById(id);

            if (employee == null)
            {
                _logger.LogError($"Employee with ID {id} not found");
                return NotFound();
            }

            return Ok(employee);
        }

        //GET search 
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchEmployees([ModelBinder(BinderType = typeof(EmployeeModelBinder))] Employee searchParams)
        {
            var employees = DataService.GetAllEmployees();

            if (!string.IsNullOrEmpty(searchParams.Name))
            {
                employees = employees.Where(e => e.Name.Contains(searchParams.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(searchParams.Position))
            {
                employees = employees.Where(e => e.Position.Contains(searchParams.Position, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (searchParams.Department != null && searchParams.Department.Id > 0)
            {
                employees = employees.Where(e => e.Department.Id == searchParams.Department.Id).ToList();
            }

            if (searchParams.StartDate != default(DateTime))
            {
                employees = employees.Where(e => e.StartDate.Date == searchParams.StartDate.Date).ToList();
            }

            if (employees.Count == 0)
            {
                _logger.LogWarning("No employee found");
                return NotFound();
            }

            return Ok(employees);
        }


        // POST action
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the employee");
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Adding a new employee");
                Services.DataService.AddEmployee(employee);

                _logger.LogInformation($"Employee with ID {employee.Id} added successfully");
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding employee");
                return BadRequest();
            }
        }

        // PUT action
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateEmployee(int id, Employee employee)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the employee");
                return BadRequest(ModelState);
            }

            if (id != employee.Id)
            {
                _logger.LogError("Given ID does not match the employee ID");
                return BadRequest();
            }

            try
            {
                _logger.LogInformation($"Updating employee with ID {id}");
                Employee existingEmployee = Services.DataService.GetEmployeeById(id);

                if (existingEmployee == null)
                {
                    _logger.LogError($"Employee with ID {id} not found");
                    return NotFound();
                }

                Services.DataService.UpdateEmployee(employee);
                _logger.LogInformation($"Employee with ID {id} updated successfully");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating employee");
                return BadRequest();
            }
        }

        // DELETE action
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteEmployee(int id)
        {
            _logger.LogInformation($"Deleting employee with ID {id}");
            var employee = Services.DataService.GetEmployeeById(id);

            if (employee == null)
            {
                _logger.LogError($"Employee with ID {id} not found");
                return NotFound();
            }

            Services.DataService.DeleteEmployeeById(id);
            _logger.LogInformation($"Employee with ID {id} deleted successfully");

            return Ok();
        }

        // Assign Employee to Department
        [HttpPatch("{employeeId}/assignDepartment/{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AssignEmployeeToDepartment(int employeeId, int departmentId)
        {
            try
            {
                _logger.LogInformation($"Assigning employee with ID {employeeId} to department with ID {departmentId}");

                var employee = Services.DataService.GetEmployeeById(employeeId);
                if (employee == null)
                {
                    _logger.LogError($"Employee with ID {employeeId} not found");
                    return NotFound();
                }

                var department = Services.DataService.GetDepartmentById(departmentId);
                if (department == null)
                {
                    _logger.LogError($"Department with ID {departmentId} not found");
                    return NotFound();
                }

                employee.Department = department;
                Services.DataService.UpdateEmployee(employee);

                _logger.LogInformation($"Employee with ID {employeeId} assigned to department with ID {departmentId}");
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning employee to department");
                return BadRequest(ex.Message);
            }
        }

        // Remove Employee from Department
        [HttpPatch("{employeeId}/removeDepartment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult RemoveEmployeeFromDepartment(int employeeId)
        {
            try
            {
                _logger.LogInformation($"Removing department from employee with ID {employeeId}");

                var employee = Services.DataService.GetEmployeeById(employeeId);
                if (employee == null)
                {
                    _logger.LogError($"Employee with ID {employeeId} not found");
                    return NotFound();
                }

                employee.Department = null;
                Services.DataService.UpdateEmployee(employee);

                _logger.LogInformation($"Department removed from employee with ID {employeeId}");
                return Ok(employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing department from employee");
                return BadRequest(ex.Message);
            }
        }
    }
}
