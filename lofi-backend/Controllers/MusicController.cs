using Microsoft.AspNetCore.Mvc;
using lofi_backend.Data_Models;
using lofi_backend.Models;
using lofi_backend.Service;
using Microsoft.AspNetCore.Authorization;

namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController : ControllerBase
    {

        private readonly IMusicService _service;
        public MusicController(IMusicService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet]
        [Route("all")]
        public IActionResult GetAllMusics()
        {
            try
            {
                var result = _service.GetAllMusics();
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetMusicById(int id)
        {
            try
            {
                var result = _service.GetMusicById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateMusic([FromBody] Music music)
        {
            try
            {
                var newMusic = _service.CreateMusic(music);
                return Ok(newMusic);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult RemoveMusic(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id");
            }

            Music deletedMusic = _service.RemoveMusic(id);

            if (deletedMusic == null)
            {
                return NotFound("Music not found");
            }
            return NoContent();
        }
    }
}
