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

        [Authorize]
        [HttpGet]
        public IActionResult GetAllProjects()
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            try
            {
                var ownProjects = _projectService.GetAllProjects().Where(p => p.UserId == currentUserId);
                return Ok(ownProjects);
            }
            catch (Exception ex)
            {
                return NotFound("No projects were found");
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetProject(int id)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            Project project;
            try
            {
                project = _projectService.GetProject(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }

            if (project.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(project);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject(Project project)
        {
            var currentUserId = User.FindFirst("sub")?.Value;
            project.UserId = currentUserId;

            var newProject = await _projectService.CreateProject(project);
            return Created("", newProject);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Project Id does not exist");
            }

            var currentUserId = User.FindFirst("sub")?.Value;

            Project existingProject;
            try
            {
                existingProject = _projectService.GetProject(id);
            }
            catch (Exception)
            {
                return NotFound("Project not found");
            }

            if (existingProject.UserId != currentUserId)
            {
                return Forbid();
            }

            var projectToDelete = await _projectService.DeleteProject(id);
            if(projectToDelete == null)
            {
                return NotFound("Project not found");
            }

            return Ok(projectToDelete);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditProject([FromBody] Project project)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            Project existingProject;
            try
            {
                existingProject = _projectService.GetProject(project.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (existingProject.UserId != currentUserId)
            {
                return Forbid();
            }

            project.UserId = existingProject.UserId;

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
