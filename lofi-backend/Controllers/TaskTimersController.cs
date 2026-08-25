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

        public TaskTimersController(ITaskTimerService taskTimerService)
        {
            _taskTimerService = taskTimerService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetTimerByTimerId(int id)
        {
            try
            {
                var result = _taskTimerService.GetTimerByTimerId(id);
                return Ok(result);
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

        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewTimer([FromBody]TaskTimer taskTimer)
        {
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
            try
            {
                var result = _taskTimerService.EditTimer(timer);
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

            try
            {
                var deleteTimer = await _taskTimerService.DeleteTimer(id);
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
