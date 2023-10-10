using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class SectionRepository : RepositoryBase<Section>, ISectionRepository
{
    private readonly CalendarDbContext _db;
    public SectionRepository(CalendarDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task OverrideSections(Guid eventId, List<Section> entities)
    {
        var query = _dbSet.Where(z => z.EventId == eventId);
        _dbSet.RemoveRange(query);

        entities.ForEach(entity => entity.EventId = eventId);
        await CreateRangeAsync(entities);
    }

    public async Task CancelSections(Guid eventId)
    {
        var entities = await _dbSet.Where(z => z.EventId == eventId).ToListAsync();
        entities.ForEach(z => z.IsEnable = false);

        UpdateRange(entities);
    }
}
