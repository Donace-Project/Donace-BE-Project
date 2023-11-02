namespace Donace_BE_Project.Interfaces.Services
{
    public interface ILocationService
    {
        string GetAreaAsync(double longitude, double latitude);
    }
}
