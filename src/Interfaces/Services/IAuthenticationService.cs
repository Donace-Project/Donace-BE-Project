using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Interfaces.Services;
public interface IAuthenticationService
{
    Task<UserModel> RegisterAsync(string email);
}