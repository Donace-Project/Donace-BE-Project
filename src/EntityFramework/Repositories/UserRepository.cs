using Donace_BE_Project.Entities.User;
using Donace_BE_Project.EntityFramework;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;

namespace EntityFramework.Repository
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(AppDbContext db) : base(db)
        {
        }
    }
}
