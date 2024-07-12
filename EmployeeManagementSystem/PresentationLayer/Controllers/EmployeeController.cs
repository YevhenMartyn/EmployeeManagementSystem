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
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public EmployeeController(IEmployeeService employeeService,
                                  IMapper mapper)
        {
            _employeeService = employeeService;
            _mapper = mapper;
        }

        // GET all action
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEmployees([FromQuery(Name = "filterByDepartmentId")] int? departmentId,
                                                         [FromQuery(Name = "startedAfterDate")] DateTime? fromDate,
                                                         [FromQuery(Name = "startedBeforeDate")] DateTime? toDate)
        {
            IEnumerable<EmployeeDTO> employees = _mapper.Map<IList<EmployeeDTO>>(await _employeeService.GetAllAsync(departmentId, fromDate, toDate));
            return Ok(employees);
        }

        // GET by Id action
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            try
            {
                var employee = _mapper.Map<EmployeeDTO>(await _employeeService.GetByIdAsync(id));
                return Ok(employee);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }

        // GET search action
        [HttpGet("search")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchEmployees([ModelBinder(BinderType = typeof(EmployeeModelBinder))] EmployeeDTO searchParams)
        {
            try
            {
                var employees = _mapper.Map<IList<EmployeeDTO>>(await _employeeService.GetAllAsync(searchParams.Name, searchParams.Position, searchParams.DepartmentId, searchParams.StartDate));
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
        public async Task<IActionResult> AddEmployee(EmployeeDTO employee)
        {
            try
            {
                await _employeeService.CreateAsync(_mapper.Map<EmployeeModel>(employee));
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
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDTO employee)
        {
            try
            {
                await _employeeService.UpdateAsync(id, _mapper.Map<EmployeeModel>(employee));
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
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                await _employeeService.DeleteAsync(id);
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
        public async Task<IActionResult> AssignEmployeeToDepartment(int employeeId, int departmentId)
        {
            try
            {
                var employee = _mapper.Map<EmployeeDTO>(await _employeeService.GetByIdAsync(employeeId));
                employee.DepartmentId = departmentId;
                await _employeeService.UpdateAsync(employeeId, _mapper.Map<EmployeeModel>(employee));
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
        public async Task<IActionResult> RemoveEmployeeFromDepartment(int employeeId)
        {
            try
            {
                var employee = _mapper.Map<EmployeeDTO>(await _employeeService.GetByIdAsync(employeeId));
                employee.DepartmentId = null;
                await _employeeService.UpdateAsync(employeeId, _mapper.Map<EmployeeModel>(employee));
                return Ok(employee);
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message);
            }
        }
    }
}
