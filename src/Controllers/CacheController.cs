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

        [HttpGet("suggest-location")]
        public async Task<List<string>> GetSuggestLocationAsync(string key)
        {
            var a = (await _iCacheService.GetDataByKeyAsync<List<string>>(key));
            return a.Result;
        }

        [HttpGet("suggest-test")]
        public async Task<string> GetSuggestLocationAsync(string key, int pageNumber, int pageSize)
        {
            // var a = (await _iCacheService.GetListDataByKeyPagingAsync(key, pageNumber, pageSize));
            return null;
        }
    }
}
