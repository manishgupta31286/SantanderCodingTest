using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using WebApplication2.Model;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BestStoriesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public BestStoriesController(HttpClient httpClient, IMemoryCache memoryCache)
        {
            _httpClient = httpClient;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Stories>> Get(int firstN)
        {
            if (_memoryCache.TryGetValue("bestStories", out string response))
            {

            }
            else
            {
                response = await _httpClient.GetStringAsync("https://hacker-news.firebaseio.com/v0/beststories.json");

                _memoryCache.Set("bestStories", response);
            }

            var bestStories = JsonConvert.DeserializeObject<IEnumerable<string>>(response);

            var firstNBestStories = bestStories.Take(firstN);

            List<Stories> stories = new List<Stories>();

            foreach (var item in firstNBestStories)
            {
                stories.Add(await GetStories(item));
            }

            return stories;
        }

        private async Task<Stories> GetStories(string id)
        {
            if (_memoryCache.TryGetValue($"Stories{id}", out string response))
            {

            }
            else
            {
                response = await _httpClient.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json");

                _memoryCache.Set($"Stories{id}", response);
            }
            return JsonConvert.DeserializeObject<Stories>(response.ToString());
        }
    }
}
