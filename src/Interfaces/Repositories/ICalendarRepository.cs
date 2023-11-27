using Donace_BE_Project.Entities.Calendar;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface ICalendarRepository : IRepositoryBase<Calendar>
{
    Task<List<Calendar>> GetListCalendarByIdUser(Guid userId, int pageNumber, int pageSize);
    Task<List<Calendar>> GetListCalendarPagingByIdsAsync(List<Guid> ids, int pageNumber, int pageSize);
    Task<List<Calendar>> GetListCalendarAsync(Expression<Func<Calendar, bool>> precidian);
}
