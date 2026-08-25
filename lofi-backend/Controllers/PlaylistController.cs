using lofi_backend.Models;
using lofi_backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllPlaylists()
        {
            return Ok(_playlistService.GetAllPlaylists().ToList());
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetPlaylistById(string id)
        {
            try
            {
                return Ok(_playlistService.GetPlaylistById(id));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return NotFound();
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreatePlaylist([FromBody] Playlist playlist)
        {
            try
            {
                var createdPlaylist = _playlistService.CreatePlaylist(playlist);
                return CreatedAtAction(nameof(GetPlaylistById), new { id = createdPlaylist.Id }, createdPlaylist);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPut]
        public IActionResult EditPlaylist([FromBody] Playlist playlist)
        {
            try
            {
                return Ok(_playlistService.EditPlaylist(playlist));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeletePlaylist(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Playlist ID is required.");
            }

            Playlist deletedPlaylist = _playlistService.DeletePlaylist(id);

            if (deletedPlaylist == null)
            {
              return NotFound($"Playlist with ID {id} not found.");
            }

            return NoContent();
        }

    }
}
