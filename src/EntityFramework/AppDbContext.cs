using Donace_BE_Project.Entities.Authentication;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Entities.Contact;
using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.Entities.Notification;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Entities.Post;
using Donace_BE_Project.Entities.Ticket;
using Donace_BE_Project.Entities.User;
using Microsoft.EntityFrameworkCore;

namespace Donace_BE_Project.EntityFramework;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<UserTicket> UserTickets { get; set; }

    public DbSet<Post> Posts { get; set; }
    public DbSet<UserPost> UserPosts { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    public DbSet<Event> Events { get; set; }
    public DbSet<EventParticipation> EventParticipations { get; set; }
    public DbSet<Section> Sections { get; set; }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<InvitedUser> InvitedUsers { get; set; }

    public DbSet<Calendar> Calendars { get; set; }
    public DbSet<CalendarParticipation> CalendarParticipations { get; set; }

    public DbSet<VerificationCode> VerificationCodes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = Configuration.GetConnectionString("sqlServer") ?? throw new ArgumentException(("connectionString"));
        options.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });

        // Config log entity framework
        options.LogTo(Console.WriteLine);
        
        base.OnConfiguring(options);
    }
}
