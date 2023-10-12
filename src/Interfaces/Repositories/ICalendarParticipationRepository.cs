using Donace_BE_Project.Entities.Calendar;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface ICalendarParticipationRepository : IRepositoryBase<CalendarParticipation>
{
    Task<List<Guid>> GetListCalendarIdAsync(Expression<Func<CalendarParticipation, bool>> predicate);
    Task<List<Guid>> GetListUserIdOfCalendarAsync(Expression<Func<CalendarParticipation, bool>> predicate, int pageNumber, int pageSize);
}
