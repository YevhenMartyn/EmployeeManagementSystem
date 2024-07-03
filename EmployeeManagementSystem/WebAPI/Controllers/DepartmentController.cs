﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(ILogger<DepartmentController> logger)
        {
            _logger = logger;
        }

        // GET all action
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllDepartments()
        {
            _logger.LogInformation("Fetching all departments");
            var departments = Services.DataService.GetAllDepartments();
            return Ok(departments);
        }

        // GET by Id action
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDepartmentById(int id)
        {
            _logger.LogInformation($"Fetching department with ID {id}");
            var department = Services.DataService.GetDepartmentById(id);
            if (department == null)
            {
                _logger.LogError($"Department with ID {id} not found");
                return NotFound();
            }

            return Ok(department);
        }

        // POST action
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddDepartment(Department department)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the department");
                return BadRequest(ModelState);
            }

            // Check for uniqueness
            if (Services.DataService.GetAllDepartments().FirstOrDefault(d => d.Name.ToLower() == department.Name.ToLower()) != null)
            {
                _logger.LogError("Department with the same name already exists");
                ModelState.AddModelError("AlreadyExistsError", "Such department already exists");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding a new department");
            Services.DataService.AddDepartment(department);
            _logger.LogInformation($"Department with ID {department.Id} added successfully");
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
        }

        // PUT action
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateDepartment(int id, Department department)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the department");
                return BadRequest(ModelState);
            }

            if (id != department.Id)
            {
                _logger.LogError("Given ID does not match the department ID");
                return BadRequest();
            }

            _logger.LogInformation($"Updating department with ID {id}");
            Department existingDepartment = Services.DataService.GetDepartmentById(id);
            if (existingDepartment is null)
            {
                _logger.LogError($"Department with ID {id} not found");
                return NotFound();
            }

            Services.DataService.UpdateDepartment(department);
            _logger.LogInformation($"Department with ID {id} updated successfully");

            return Ok();
        }

        // DELETE action
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Deleting department with ID {id}");
            var department = Services.DataService.GetDepartmentById(id);

            if (department is null)
            {
                _logger.LogError($"Department with ID {id} not found");
                return NotFound();
            }

            Services.DataService.DeleteDepartmentById(id);
            _logger.LogInformation($"Department with ID {id} deleted successfully");

            return Ok();
        }

        // PATCH action
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialDepartment(int id, JsonPatchDocument<Department> patch)
        {
            if (patch == null || id == 0)
            {
                _logger.LogError("Invalid patch document or ID");
                return BadRequest();
            }

            _logger.LogInformation($"Patching department with ID {id}");
            var department = Services.DataService.GetAllDepartments().FirstOrDefault(d => d.Id == id);
            if (department is null)
            {
                _logger.LogError($"Department with ID {id} not found");
                return BadRequest();
            }

            patch.ApplyTo(department, ModelState);

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state after applying patch");
                return BadRequest(ModelState);
            }

            _logger.LogInformation($"Department with ID {id} patched successfully");
            return Ok();
        }
    }
}
