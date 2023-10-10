using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Identity;

namespace Donace_BE_Project.Interfaces.Services;
public interface IAuthenticationAppService
{
    Task<string> CreateTokenAsync();
    Task<LoginResponse> LoginAsync(UserDto input);
    Task<IdentityResult> RegisterAsync(UserDto input);
}