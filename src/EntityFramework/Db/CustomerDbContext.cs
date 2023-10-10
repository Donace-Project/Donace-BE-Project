using Donace_BE_Project.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Donace_BE_Project.EntityFramework.Db
{
    public class CustomerDbContext : IdentityDbContext<User>
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
