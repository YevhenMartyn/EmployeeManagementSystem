using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        // GET all action
        [HttpGet]
        public IActionResult GetAllEmployees() => Ok(Services.DataService.GetAllEmployees());

        // GET by Id action
        [HttpGet("{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            var employee = Services.DataService.GetEmployeeById(id);
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        // POST action
        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            Services.DataService.AddEmployee(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        // PUT action
        [HttpPut("{id}")]
        public IActionResult UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.Id)
                return BadRequest();

            Employee existingEmployee = Services.DataService.GetEmployeeById(id);
            if (existingEmployee is null)
                return NotFound();

            Services.DataService.UpdateEmployee(employee);

            return Ok();
        }


        // DELETE action
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var employee = Services.DataService.GetEmployeeById(id);

            if (employee is null)
                return NotFound();

            Services.DataService.DeleteEmployeeById(id);

            return Ok();
        }
    }
}
