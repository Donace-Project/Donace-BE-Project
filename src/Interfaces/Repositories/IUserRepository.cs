using Donace_BE_Project.Entities.User;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IUserRepository
    {
        ValueTask<User?> FindByIdAsync(Guid id);

        void Update(User user);

        Task<List<User>> GetListAsync(Expression<Func<User, bool>> predicate);
    }
}
