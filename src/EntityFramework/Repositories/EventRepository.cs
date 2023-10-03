using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;

namespace Donace_BE_Project.EntityFramework.Repository;

public class EventRepository : RepositoryBase<Event>, IEventRepository
{
    public EventRepository(AppDbContext db) : base(db)
    {
    }

    public override Task<Event> CreateAsync(Event entity)
    {
        var sections = entity.Sections;
        for (int i = 0; i < sections.Count; i++)
        {
            sections[i].Id = Guid.NewGuid();
            sections[i].CreationTime = DateTime.Now;
        }
        return base.CreateAsync(entity);
    }
}
