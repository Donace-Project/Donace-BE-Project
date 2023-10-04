using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Models.Event.Input;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface IEventRepository : IRepositoryBase<Event>
{
    void CancelAsync(Event entity);
    Task<(int TotalCount, List<Event> Items)> GetPaginationAsync(PaginationEventInput input);
}
