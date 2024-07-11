using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Interface;
using PresentationLayer.Models;
using AutoMapper;
using BusinessLogicLayer.Models;
using BusinessLogicLayer.Exceptions;

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
            var departments = _mapper.Map<IList<DepartmentDTO>>(_departmentService.GetAll());
            return Ok(departments);
        }

        // GET by Id action
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetDepartmentById(int id)
        {
            try
            {
                var department = _mapper.Map<DepartmentDTO>(_departmentService.GetById(id));
                return Ok(department);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        // POST action
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult AddDepartment(DepartmentDTO department)
        {
            //// Check for uniqueness
            //if (_mapper.Map<IList<DepartmentDTO>>(_departmentService.GetAll()).FirstOrDefault(d => d.Name.ToLower() == department.Name.ToLower()) != null)
            //{
            //    _logger.LogError("Department with the same name already exists");
            //    ModelState.AddModelError("AlreadyExistsError", "Such department already exists");
            //    return BadRequest(ModelState);
            //}
            try
            {
                _departmentService.Create(_mapper.Map<DepartmentModel>(department));
                return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
            }
            catch (CustomException ex) 
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        // PUT action
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateDepartment(int id, DepartmentDTO department)
        {
            try
            {
                _departmentService.Update(id, _mapper.Map<DepartmentModel>(department));
                return Ok();
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        // DELETE action
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteDepartment(int id)
        {
            try
            {
                _departmentService.Delete(id);
                return Ok();
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }
    }
}
