using lofi_backend.Service;
using Microsoft.AspNetCore.Mvc;


namespace lofi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YoutubeController : ControllerBase
    {
        private readonly IYoutubeService _youtubeService;
        public YoutubeController(IYoutubeService youtubeService)
        {
            _youtubeService = youtubeService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchYoutubeAsync(string search = "")
        {
            Console.WriteLine("Something");
            try
            {
                Console.WriteLine("Attempting to get results");
                var results = await _youtubeService.SearchYoutubeAsync(search);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest();
            }
        }
    }
}
