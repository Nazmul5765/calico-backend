using lofi_backend.Models;
using lofi_backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskTimersController : ControllerBase
    {
        private readonly ITaskTimerService _taskTimerService;
        private readonly IProjectService _projectService;

        public TaskTimersController(ITaskTimerService taskTimerService, IProjectService projectService)
        {
            _taskTimerService = taskTimerService;
            _projectService = projectService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetTimerByTimerId(int id)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            TaskTimer timer;
            try
            {
                timer = _taskTimerService.GetTimerByTimerId(id);
            }
            catch (Exception ex)
            {
                if (id <= 0)
                {
                    return BadRequest(ex.Message);
                }
                else
                {
                    return NotFound("Timer not found");
                }
            }

            Project project;
            try
            {
                project = _projectService.GetProject(timer.ProjectId);
            }
            catch (Exception)
            {
                return NotFound("Timer not found");
            }

            if (project.UserId != currentUserId)
            {
                return Forbid();
            }

            return Ok(timer);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewTimer([FromBody]TaskTimer taskTimer)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            Project project;
            try
            {
                project = _projectService.GetProject(taskTimer.ProjectId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (project.UserId != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var newTimer = await _taskTimerService.CreateNewTimer(taskTimer);
                return Created("", newTimer);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditTimer([FromBody] TaskTimer timer)
        {
            var currentUserId = User.FindFirst("sub")?.Value;

            TaskTimer existingTimer;
            try
            {
                existingTimer = _taskTimerService.GetTimerByTimerId(timer.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            Project project;
            try
            {
                project = _projectService.GetProject(existingTimer.ProjectId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            if (project.UserId != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var result = await _taskTimerService.EditTimer(timer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteTimer(int id)
        {
            if(id <= 0)
            {
                return BadRequest("Timer id must be greater than zero.");

            }

            var currentUserId = User.FindFirst("sub")?.Value;

            TaskTimer existingTimer;
            try
            {
                existingTimer = _taskTimerService.GetTimerByTimerId(id);
            }
            catch (Exception ex)
            {
                return NotFound($"Timer with id {id} was not found., {ex.Message}");
            }

            Project project;
            try
            {
                project = _projectService.GetProject(existingTimer.ProjectId);
            }
            catch (Exception)
            {
                return NotFound($"Timer with id {id} was not found.");
            }

            if (project.UserId != currentUserId)
            {
                return Forbid();
            }

            try
            {
                await _taskTimerService.DeleteTimer(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound($"Timer with id {id} was not found., {ex.Message}" );
            }
        }

        [Authorize]
        [HttpGet("Projects")]

        public async Task<IActionResult> GetTimerByProjectId(int projectId)
        {
            if (projectId <= 0)
            {
                return BadRequest("project id must be greater than zero.");
            }

            var currentUserId = User.FindFirst("sub")?.Value;

            Project project;
            try
            {
                project = _projectService.GetProject(projectId);
            }
            catch (Exception ex)
            {
                return NotFound($"Project not found by projectId, {ex.Message}");
            }

            if (project.UserId != currentUserId)
            {
                return Forbid();
            }

            try
            {
                var result = await _taskTimerService.GetAllTimersByProjectId(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {

                    return NotFound($"Project not found by projectId, {ex.Message}");

            }
        }
    }
}
