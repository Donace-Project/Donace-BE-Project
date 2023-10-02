using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Interfaces.Services;
public interface IUserService
{
    Task<UserModel> GetProfileAsync(Guid id);
}