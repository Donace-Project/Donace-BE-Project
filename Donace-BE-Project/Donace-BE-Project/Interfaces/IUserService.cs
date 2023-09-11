using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Interfaces;
public interface IUserService
{
    Task<UserModel> GetProfileAsync(Guid id);
    Task<UserModel> RegisterAsync(string email);
}