using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;

namespace Donace_BE_Project.EntityFramework.Repository;

public class EventRepository : RepositoryBase<Event>, IEventRepository
{
    public EventRepository(AppDbContext db) : base(db)
    {
    }
}
