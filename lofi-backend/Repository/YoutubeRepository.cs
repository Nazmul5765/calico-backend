using System.Text.Json;
using lofi_backend.Data_Models.Enums;
using lofi_backend.Models;
using Microsoft.Extensions.Caching.Memory;

namespace lofi_backend.Repository
{
    public interface IYoutubeRepository
    {
        Task<List<Music>> SearchYoutubeAsync(string search);

    }

    public class YoutubeRepository : IYoutubeRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;


        public YoutubeRepository(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<List<Music>> SearchYoutubeAsync(string search)
        {
            var cacheKey = $"youtube-search:{search.ToLowerInvariant()}";

            if (_cache.TryGetValue(cacheKey, out List<Music> cachedResults))
            {

                Console.WriteLine($"Cache HIT for '{search}'");
                return cachedResults;
            }

            Console.WriteLine($"Cache MISS for '{search}' — calling YouTube API");
            var apiKey = _configuration["YouTube:ApiKey"];

            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("YouTube API key is missing.");
            }

            var client = _httpClientFactory.CreateClient();

            var url =
                "https://youtube.googleapis.com/youtube/v3/search" +
                "?part=snippet" +
                $"&q={Uri.EscapeDataString(search + " lofi music")}" +
                "&maxResults=20" +
                "&order=viewCount" +
                $"&key={apiKey}";

            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            using var document = JsonDocument.Parse(json);

            var musicList = new List<Music>();

            foreach (var item in document.RootElement.GetProperty("items").EnumerateArray())
            {
                var snippet = item.GetProperty("snippet");
                var title = snippet.GetProperty("title").GetString() ?? "Unknown Title";
                var channel = snippet.GetProperty("channelTitle").GetString() ?? "Unknown Channel";
                var videoId = item.GetProperty("id").GetProperty("videoId").GetString() ?? string.Empty;


                musicList.Add(new Music
                {
                    Id = musicList.Count + 1,
                    Title = title ?? "",
                    Artist = channel ?? "",
                    Channel = channel ?? "",
                    Mood = Mood.Chill,
                    Genre = Genre.LoFi,
                    URL = $"https://www.youtube.com/watch?v={videoId}",
                    Thumbnail = snippet.GetProperty("thumbnails").GetProperty("default").GetProperty("url").GetString() ?? ""
                });
            }
            _cache.Set(cacheKey, musicList, TimeSpan.FromMinutes(15));
            return musicList;
        }
    }
}
