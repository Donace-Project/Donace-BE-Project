using Donace_BE_Project.Entities.Authentication;
using Donace_BE_Project.Entities.Contact;
using Donace_BE_Project.Entities.Notification;
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

        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<InvitedUser> InvitedUsers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }
}
