using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        // GET all action
        [HttpGet]
        public IActionResult GetAllDepartments() => Ok(Services.DataService.GetAllDepartments());

        // GET by Id action
        [HttpGet("{id}")]
        public IActionResult GetDepartmentById(int id)
        {
            var department = Services.DataService.GetDepartmentById(id);
            if (department == null)
                return NotFound();

            return Ok(department);
        }

        // POST action


        // PUT action

        // DELETE action
    }
}
