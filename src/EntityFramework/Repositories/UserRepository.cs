using Donace_BE_Project.Entities.User;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.Interfaces.Repositories;

namespace EntityFramework.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly CustomerDbContext _dbContext;
        public UserRepository(CustomerDbContext customerDb) 
        {
            _dbContext = customerDb;
        }

        public ValueTask<User?> FindByIdAsync(Guid id)
        {
            return _dbContext.Users.FindAsync(id.ToString());
        }
    }
}
