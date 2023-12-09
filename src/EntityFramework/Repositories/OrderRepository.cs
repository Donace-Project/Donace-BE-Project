using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;

namespace Donace_BE_Project.EntityFramework.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(CalendarDbContext db, IUserProvider userProvider) : base(db, userProvider)
        {
        }
    }
}
