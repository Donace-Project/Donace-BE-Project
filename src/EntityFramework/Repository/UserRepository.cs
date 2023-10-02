using Donace_BE_Project.Entities.User;
using Donace_BE_Project.EntityFramework;

namespace EntityFramework.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db)
        {
        }
    }
}
