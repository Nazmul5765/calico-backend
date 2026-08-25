using lofi_backend.Data_Models;
using lofi_backend.Models;
using lofi_backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProjectsController: ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public IActionResult GetAllProjects()
        {
            try
            {
                return Ok(_projectService.GetAllProjects());
            }
            catch (Exception ex)
            {
                return NotFound("No projects were found");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetProject(int id)
        {
            try
            {
                return Ok(_projectService.GetProject(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }

        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject(Project project)
        {
            var newProject = await _projectService.CreateProject(project);
            return Created("", newProject);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Project Id does not exist");
            }

            var projectToDelete = await _projectService.DeleteProject(id);
            if(projectToDelete == null)
            {
                return NotFound("Project not found");
            }

            return Ok(projectToDelete);
        }

        [HttpPut]
        public async Task<IActionResult> EditProject([FromBody] Project project)
        {
            try
            {
                var projectToEdit = await _projectService.EditProject(project);
                return  Ok(projectToEdit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
