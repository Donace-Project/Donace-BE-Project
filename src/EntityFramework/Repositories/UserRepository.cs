using Donace_BE_Project.Entities.User;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public async Task<List<User>> GetListAsync(Expression<Func<User, bool>> predicate)
        {
            return await _dbContext.Users.Where(predicate).ToListAsync();
        }

        public void Update(User user)
        {
            _dbContext.Users.Update(user);
        }
    }
}
