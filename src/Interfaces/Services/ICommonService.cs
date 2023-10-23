namespace Donace_BE_Project.Interfaces.Services
{
    public interface ICommonService
    {
        string UpLoadImageAsync(IFormFile file, Guid eventId);
    }
}
