using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Cache;
using System;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface ICacheService
    {
        Task<ResponseModel<T>> GetDataByKeyAsync<T>(string key);
        Task<ResponseModel<T>> SetDataAsync<T>(string key, T value);
        Task<bool> RemoveDataAsync<T>(string key);

        Task<ResponseModel<List<T>>> GetListDataByKeyPagingAsync<T>(string key, int pageNumber, int pageSize);
        Task SetDataSortedAsync<T>(string key, List<T> value) where T : CacheSortedBaseModel;
        Task RemoveItemDataBySortedAsync(string key, double sorted);
        Task UpdateValueScoreAsync<T>(string key, double score, T value);
        Task<List<string>> GetListDataByScoreAsync(string key, double score);
        Task<List<T>> GetListDataByKeyRangeIdAsync<T>(string key);

    }
}
