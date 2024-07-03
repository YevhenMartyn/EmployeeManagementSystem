using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;

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
        [HttpPost]
        public IActionResult AddDepartment(Department department)
        {
            Services.DataService.AddDepartment(department);
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
        }

        // PUT action
        [HttpPut("{id}")]
        public IActionResult UpdateDepartment(int id, Department department)
        {
            if (id != department.Id)
                return BadRequest();

            Department existingDepartment = Services.DataService.GetDepartmentById(id);
            if (existingDepartment is null)
                return NotFound();

            Services.DataService.UpdateDepartment(department);

            return Ok();
        }


        // DELETE action
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var department = Services.DataService.GetDepartmentById(id);

            if (department is null)
                return NotFound();

            Services.DataService.DeleteDepartmentById(id);

            return Ok();
        }
    }
}