using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Models.Event.Input;
using EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;

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

    public async Task<(int TotalCount, List<Event> Items)> GetPaginationAsync(PaginationEventInput input)
    {
        var query = _dbSet
            // TODO: Filter fromDate & toDate 
            //.Where()
            .Where(z => z.IsEnable == true)
            //.Include(z => z.Sections)
            .GetPagination(input.PageNumber, input.PageSize);

        var totalCount = await query.CountAsync();
        var results = await query.ToListAsync();
        return new(totalCount, results);
    }

    public Task<Event?> GetDetailById(Guid id)
    {
        return _dbSet
            .Include(z => z.Sections)
            .FirstOrDefaultAsync(z => z.Id == id);
    }

    public void CancelAsync(Event entity)
    {
        entity.IsEnable = false;
    }
}
