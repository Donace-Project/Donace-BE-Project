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
        Task<ResponseModel<string>> GetListDataByKeyPagingAsync(string key, int pageNumber, int pageSize);
        Task SetListDataSortedAsync<T>(string key, T value) where T : CacheSortedBaseModel;
    }
}
