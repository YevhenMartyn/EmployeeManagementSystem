﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

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
        public IActionResult GetAllEmployees([FromQuery(Name = "filterByDepartmentName")] string? departmentName)
        {
            _logger.LogInformation("Fetching all employees");

            IEnumerable<Employee> employees;
            if (departmentName is not null)
            {
                employees = Services.DataService.GetAllEmployees().Where(n => n.Department.Name == departmentName);
            }
            else
            {
                employees = Services.DataService.GetAllEmployees();
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
                if (existingEmployee is null)
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

            if (employee is null)
            {
                _logger.LogError($"Employee with ID {id} not found");
                return NotFound();
            }

            Services.DataService.DeleteEmployeeById(id);
            _logger.LogInformation($"Employee with ID {id} deleted successfully");

            return Ok();
        }

        // PATCH action
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialEmployee(int id, JsonPatchDocument<Employee> patch)
        {
            if (patch == null || id == 0)
            {
                _logger.LogError("Invalid patch document or ID");
                return BadRequest();
            }

            _logger.LogInformation($"Patching employee with ID {id}");
            var employee = Services.DataService.GetAllEmployees().FirstOrDefault(d => d.Id == id);
            if (employee is null)
            {
                _logger.LogError($"Employee with ID {id} not found");
                return BadRequest();
            }

            patch.ApplyTo(employee, ModelState);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state after applying patch");
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Employee with ID {id} patched successfully");
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
