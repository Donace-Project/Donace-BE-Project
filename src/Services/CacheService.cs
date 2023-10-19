using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Donace_BE_Project.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _iDistributeCache;
        private readonly ILogger<CacheService> _iLogger;
        private readonly IUserProvider _iUserProvider;
        public CacheService(IDistributedCache distributeCache,
                            ILogger<CacheService> iLogger,
                            IUserProvider iUserProvider)
        {
            _iDistributeCache = distributeCache;
            _iLogger = iLogger;
            _iUserProvider = iUserProvider;

        }
        public async Task<ResponseModel<T>> GetDataByKeyAsync<T>(string key)
        {
            try
            {
                
                var data = await _iDistributeCache.GetStringAsync($"{key}:{_iUserProvider.GetUserId()}");
                if(string.IsNullOrEmpty(data))
                {
                    return new ResponseModel<T>(true, "200");
                }
                var result = JsonConvert.DeserializeObject<T>(data);

                return new ResponseModel<T>(true, "200", result);
            }
            catch (Exception ex)
            {
                _iLogger.LogError($"CacheService.Exception: {ex.Message}", $"{key}:{_iUserProvider.GetUserId()}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CacheService, $"{JsonConvert.SerializeObject(new { key })}");
            }
        }

        public async Task<bool> RemoveDataAsync<T>(string key)
        {
            try
            {
                await _iDistributeCache.RemoveAsync($"{key}:{_iUserProvider.GetUserId()}");
                return true;
            }
            catch(Exception ex)
            {
                _iLogger.LogError($"CacheService.Exception: {ex.Message}", $"{key}:{_iUserProvider.GetUserId()}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CacheService, $"{JsonConvert.SerializeObject(new { key })}");
            }
        }

        public async Task<ResponseModel<T>> SetDataAsync<T>(string key, T value)
        {
            try
            {
                key = $"{key}:{_iUserProvider.GetUserId()}";
                var strData = JsonConvert.SerializeObject(value);
                await _iDistributeCache.SetStringAsync(key, strData);
                return new ResponseModel<T>(true, "200", value);
            }
            catch(Exception ex )
            {
                _iLogger.LogError($"CacheService.Exception: {ex.Message}", $"{key}:{_iUserProvider.GetUserId()}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CacheService, $"{JsonConvert.SerializeObject(new { key })}");
            }
        }
    }
}
