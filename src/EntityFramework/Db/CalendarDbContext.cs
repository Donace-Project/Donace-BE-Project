using Donace_BE_Project.Entities.Authentication;
using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Entities.Contact;
using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.Entities.Notification;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Entities.Post;
using Donace_BE_Project.Entities.Ticket;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Donace_BE_Project.EntityFramework.Db;

public class CalendarDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public CalendarDbContext(DbContextOptions<CalendarDbContext> options, IConfiguration configuration) : base(options)
    {
        Configuration = configuration;
    }

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
        var connectionString = Configuration.GetConnectionString("calendar") ?? throw new ArgumentException("connectionString");
        options.UseSqlServer(connectionString, builder =>
        {
            builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        });

        // Config log entity framework
        options.LogTo(Console.WriteLine);

        base.OnConfiguring(options);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Áp dụng global query filter cho tất cả các entity kế thừa từ BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Áp dụng global query filter cho mỗi entity
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GenerateQueryFilterLambdaExpression(entityType.ClrType));
            }
        }
    }

    #region Privite Methods
    private LambdaExpression GenerateQueryFilterLambdaExpression(Type type)
    {
        // we should generate:  e => e.IsDeleted == false
        // or: e => !e.IsDeleted

        // e =>
        var parameter = Expression.Parameter(type, "e");

        // false
        var falseConstant = Expression.Constant(false);

        // e.IsDeleted
        var propertyAccess = Expression.PropertyOrField(parameter, nameof(BaseEntity.IsDeleted));

        // e.IsDeleted == false
        var equalExpression = Expression.Equal(propertyAccess, falseConstant);

        // e => e.IsDeleted == false
        var lambda = Expression.Lambda(equalExpression, parameter);

        return lambda;
    }
    #endregion
}
