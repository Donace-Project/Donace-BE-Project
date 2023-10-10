using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Identity;

namespace Donace_BE_Project.Interfaces.Services;
public interface IUserService
{
    Task<UserModel> GetProfileAsync(Guid id);
    Task<LoginResponse> LoginAsync(UserDto input);
    Task<IdentityResult> RegisterAsync(UserDto input);
}