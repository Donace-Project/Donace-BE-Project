using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Event.Input;
using EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Donace_BE_Project.EntityFramework.Repository;

public class EventRepository : RepositoryBase<Event>, IEventRepository
{
    public EventRepository(CalendarDbContext db, IUserProvider userProvider) : base(db, userProvider)
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

    public async Task<List<Event>> GetPaginationAsync(PaginationEventInput input, Guid userId)
    {
        try
        {
            var query = input.IsNew == null ? await _dbSet.Where(x => x.IsDeleted == false && x.CreatorId == userId).ToListAsync() :
            input.IsNew.Value ?
                        await _dbSet.Where(x => x.EndDate >= DateTime.Now && x.IsDeleted == false && x.CreatorId == userId).ToListAsync() :
                        await _dbSet.Where(x => x.EndDate < DateTime.Now && x.IsDeleted == false && x.CreatorId == userId).ToListAsync();

            return query;
        }
        catch(Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public Task<Event?> GetDetailBySorted(int sorted)
    {
        return _dbSet
            .Include(z => z.Sections)
            .FirstOrDefaultAsync(z => z.Sorted == sorted);
    }

    public Task<Event?> GetDetailByIdAsync(Guid id)
    {
        return _dbSet
            .Include(z => z.Sections)
            .FirstOrDefaultAsync(z => z.Id == id);
    }

    public void CancelAsync(Event entity)
    {
        entity.IsEnable = false;
        _dbSet.Update(entity);
    }

    public async Task<List<Event>> GetListAsync(Expression<Func<Event, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
}
