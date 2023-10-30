using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Interfaces.Services;
public interface IAuthenticationAppService
{
    Task<string> CreateTokenAsync();
    Task<LoginResponse> LoginAsync(UserDto input);
    Task<RegisterResponse> RegisterAsync(UserDto input);
}