using Donace_BE_Project.Models.Eto;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IHttpClientService
    {
        Task<T> CallApiPost<T>(string domain, string endpoint, T data) where T : BaseEto;
    }
}
