using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Donace_BE_Project.EntityFramework.Repositories;

public class CalendarRepository : RepositoryBase<Calendar>, ICalendarRepository
{
    public CalendarRepository(CalendarDbContext db, IUserProvider userProvider) : base(db, userProvider)
    {
    }

    public async Task<List<Calendar>> GetListCalendarByIdUser(Guid userId, int pageNumber, int pageSize)
    {
        return await _dbSet.Where(x => x.CreatorId.Equals(userId))
                           .Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .OrderByDescending(x => x.Sorted)
                           .ToListAsync();
    }

    public async Task<List<Calendar>> GetListCalendarPagingByIdsAsync(List<Guid> ids, int pageNumber, int pageSize)
    {
        return await _dbSet.Where(x => ids.Contains(x.Id))
                           .Skip((pageNumber -1) * pageSize)
                           .Take(pageSize)
                           .OrderByDescending(x => x.Sorted)
                           .ToListAsync();
    }
}
