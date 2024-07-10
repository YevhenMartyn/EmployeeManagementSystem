using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Interface;
using PresentationLayer.Models;
using AutoMapper;
using BusinessLogicLayer.Models;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly ILogger<DepartmentController> _logger;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;
        public DepartmentController(
            ILogger<DepartmentController> logger,
            IDepartmentService departmentService,
            IMapper mapper)
        {
            _logger = logger;
            _departmentService = departmentService;
            _mapper = mapper;
        }

        // GET all action
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllDepartments()
        {
            _logger.LogInformation("Fetching all departments");
            var departments = _mapper.Map<IList<DepartmentDTO>>(_departmentService.GetAll());
            return Ok(departments);
        }

        // GET by Id action
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDepartmentById(int id)
        {
            _logger.LogInformation($"Fetching department with ID {id}");
            var department = _mapper.Map<DepartmentDTO>(_departmentService.GetById(id));

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
        public IActionResult AddDepartment(DepartmentDTO department)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid model state for the department");
                return BadRequest(ModelState);
            }

            // Check for uniqueness
            if (_mapper.Map<IList<DepartmentDTO>>(_departmentService.GetAll()).FirstOrDefault(d => d.Name.ToLower() == department.Name.ToLower()) != null)
            {
                _logger.LogError("Department with the same name already exists");
                ModelState.AddModelError("AlreadyExistsError", "Such department already exists");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Adding a new department");
            _departmentService.Create(_mapper.Map<Department>(department));
            _logger.LogInformation($"Department with ID {department.Id} added successfully");
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
        }

        // PUT action
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateDepartment(int id, DepartmentDTO department)
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
            DepartmentDTO existingDepartment = _mapper.Map<DepartmentDTO>(_departmentService.GetById(id));
            if (existingDepartment == null)
            {
                _logger.LogError($"Department with ID {id} not found");
                return NotFound();
            }

            _departmentService.Update(_mapper.Map<Department>(department));
            _logger.LogInformation($"Department with ID {id} updated successfully");

            return Ok();
        }

        // DELETE action
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteDepartment(int id)
        {
            _logger.LogInformation($"Deleting department with ID {id}");
            var department = _mapper.Map<DepartmentDTO>(_departmentService.GetById(id));

            if (department == null)
            {
                _logger.LogError($"Department with ID {id} not found");
                return NotFound();
            }

            _departmentService.Delete(id);
            _logger.LogInformation($"Department with ID {id} deleted successfully");

            return Ok();
        }
    }
}
