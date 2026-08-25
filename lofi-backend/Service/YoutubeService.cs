using lofi_backend.Models;
using lofi_backend.Repository;

namespace lofi_backend.Service
{

    public interface IYoutubeService
    {
        Task<List<Music>> SearchYoutubeAsync(string search);
    }
    public class YoutubeService : IYoutubeService
    {
        private readonly IYoutubeRepository _youtubeRepository;
        public YoutubeService(IYoutubeRepository youtubeRepository)
        {
            _youtubeRepository = youtubeRepository;
        }
        public async Task<List<Music>> SearchYoutubeAsync(string search)
        {
            Console.WriteLine("Service layer");
            return await _youtubeRepository.SearchYoutubeAsync(search);
        }
    }
}
