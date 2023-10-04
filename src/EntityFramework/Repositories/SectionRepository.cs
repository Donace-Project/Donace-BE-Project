using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class SectionRepository : RepositoryBase<Section>, ISectionRepository
{
    private readonly AppDbContext _db;
    public SectionRepository(AppDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task DeleteAll_By_EventIdAsync(Guid eventId)
    {
        var query = _dbSet.Where(z => z.EventId == eventId);
        _dbSet.RemoveRange(query);
        await _db.SaveChangesAsync();
    }

    public async Task OverrideSections(Guid eventId, List<Section> entities)
    {
        var query = _dbSet.Where(z => z.EventId == eventId);
        _dbSet.RemoveRange(query);
        
        entities.ForEach(entity => entity.EventId = eventId);
        await CreateRangeAsync(entities);
    }
}
