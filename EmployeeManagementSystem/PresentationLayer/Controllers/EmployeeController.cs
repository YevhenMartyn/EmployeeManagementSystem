using AutoMapper;
using BusinessLogicLayer.Exceptions;
using BusinessLogicLayer.Interface;
using BusinessLogicLayer.Models;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ModelBinders;
using PresentationLayer.Models;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService, IMapper mapper)
        {
            _logger = logger;
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET all action
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllEmployees([FromQuery(Name = "filterByDepartmentId")] int? departmentId,
                                             [FromQuery(Name = "startedAfterDate")] DateTime? fromDate,
                                             [FromQuery(Name = "startedBeforeDate")] DateTime? toDate)
        {
            IEnumerable<EmployeeDTO> employees = _mapper.Map<IList<EmployeeDTO>>(_employeeService.GetAll(departmentId, fromDate, toDate));

            return Ok(employees);
        }

        // GET by Id action
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEmployeeById(int id)
        {
            try
            {
                var employee = _mapper.Map<EmployeeDTO>(_employeeService.GetById(id));
                return Ok(employee);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        //GET search 
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult SearchEmployees([ModelBinder(BinderType = typeof(EmployeeModelBinder))] EmployeeDTO searchParams)
        {
            try
            {
                var employees = _mapper.Map<IList<EmployeeDTO>>(_employeeService.GetAll(searchParams.Name, searchParams.Position, searchParams.DepartmentId, searchParams.StartDate));
                return Ok(employees);
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
        public IActionResult AddEmployee(EmployeeDTO employee)
        {
            try
            {
                _employeeService.Create(_mapper.Map<EmployeeModel>(employee));
                return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
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
        public IActionResult UpdateEmployee(int id, EmployeeDTO employee)
        {
            try
            {
                _employeeService.Update(id, _mapper.Map<EmployeeModel>(employee));
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
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                _employeeService.Delete(id);
                return Ok();
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
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
                var employee = _mapper.Map<EmployeeDTO>(_employeeService.GetById(employeeId));
                employee.DepartmentId = departmentId;
                _employeeService.Update(employeeId, _mapper.Map<EmployeeModel>(employee));
                return Ok(employee);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
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
                var employee = _mapper.Map<EmployeeDTO>(_employeeService.GetById(employeeId));
                employee.DepartmentId = null;
                _employeeService.Update(employeeId, _mapper.Map<EmployeeModel>(employee));
                return Ok(employee);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }
    }
}
