using Donace_BE_Project.Entities.User;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IUserRepository
    {
        ValueTask<User?> FindByIdAsync(Guid id);
    }
}
