using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Donace_BE_Project.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _iDistributeCache;
        private readonly ILogger<CacheService> _iLogger;
        private readonly IUserProvider _iUserProvider;
        private readonly IConnectionMultiplexer _iConnectionMultiplexer;
        public CacheService(IDistributedCache distributeCache,
                            ILogger<CacheService> iLogger,
                            IUserProvider iUserProvider,
                            IConnectionMultiplexer iConnectionMultiplexer)
        {
            _iDistributeCache = distributeCache;
            _iLogger = iLogger;
            _iUserProvider = iUserProvider;
            _iConnectionMultiplexer = iConnectionMultiplexer;
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

        public async Task<ResponseModel<string>> GetListDataByKeyPagingAsync(string key, int pageNumber, int pageSize)
        {
            try
            {
                var db = _iConnectionMultiplexer.GetDatabase(0);                
                int start = (pageNumber - 1) * pageSize;
                int stop = start + pageSize - 1;

                SortedSetEntry[] pagedData = db.SortedSetRangeByRankWithScores(key, start, stop, Order.Descending);

                if(!pagedData.Any())
                {
                    return new ResponseModel<string>(true, "204", string.Empty);
                }
                var strResult = JsonConvert.SerializeObject(pagedData.Select(x => x.Element));
                return new ResponseModel<string>(true, "200", strResult);
            }
            catch (Exception ex)
            {
                _iLogger.LogError($"CacheService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(new { key, pageNumber, pageSize })}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CacheService, ex.Message);
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

        public async Task SetListDataAsync<T>(string key, T value)
        {
            try
            {
                var db = _iConnectionMultiplexer.GetDatabase();

                var maxScoreValue = db.SortedSetRangeByScoreWithScores(key,
                                                                           double.PositiveInfinity,
                                                                           double.NegativeInfinity,
                                                                           Exclude.None,
                                                                           Order.Descending, 0, 1).FirstOrDefault();

                if (value is List<T>)
                {
                    List<T> listValue = (List<T>)Convert.ChangeType(value, typeof(List<T>));                                        

                    for(int i = 0; i <= listValue.Count; i++)
                    {
                        string data = JsonConvert.SerializeObject(listValue[i]);

                        db.SortedSetAdd(key, data, i + maxScoreValue.Score);
                    }

                    return;
                }

                string jsonData = JsonConvert.SerializeObject(value);

                db.SortedSetAdd(key, jsonData, maxScoreValue.Score + 1);

                return;
            }
            catch(Exception ex)
            {
                _iLogger.LogError($"CacheService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(value)}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CacheService, ex.Message);
            }
        }
    }
}
