using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Interfaces;
public interface IAuthenticationService
{
    Task<UserModel> RegisterAsync(string email);
}