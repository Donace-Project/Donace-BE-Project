namespace Donace_BE_Project.Interfaces.Services;

public interface IUserProvider
{
    Guid GetUserId();
    string GetEmailUser();
}