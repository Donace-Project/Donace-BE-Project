namespace Donace_BE_Project.Interfaces.Services
{
    public interface ICommonService
    {
        Task<string> UpLoadImageAsync(IFormFile file, Guid eventId);
    }
}
