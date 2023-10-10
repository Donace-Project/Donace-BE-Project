using Donace_BE_Project.Entities.Calendar;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface ICalendarParticipationRepository : IRepositoryBase<CalendarParticipation>
{
    Task<CalendarParticipation> FindAsync(Expression<Func<CalendarParticipation, bool>> predicate);
}
