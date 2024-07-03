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


        // PUT action

        // DELETE action
    }
}
