using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Models.Event.Input;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface IEventRepository : IRepositoryBase<Event>
{
    Task<Event?> GetDetailById(Guid id);
    Task<(int TotalCount, List<Event> Items)> GetPaginationAsync(PaginationEventInput input);
    void CancelAsync(Event entity);
    Task CreateAsync(EventCreateInput eventEntity);
}
