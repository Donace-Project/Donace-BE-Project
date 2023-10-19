using Donace_BE_Project.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly ICacheService _iCacheService;
        public CacheController(ICacheService cacheService)
        {
            _iCacheService = cacheService;
        }

        [HttpGet]
        public async Task GetSuggestLocationAsync(string key)
        {
            await _iCacheService.GetDataByKeyAsync<List<string>>(key);
        }
    }
}
