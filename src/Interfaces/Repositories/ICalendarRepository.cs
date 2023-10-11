using Donace_BE_Project.Entities.Calendar;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface ICalendarRepository : IRepositoryBase<Calendar>
{
    Task<List<Calendar>> GetListCalendarByIds(List<Guid> Id, int pageNumber, int pageSize);
}
