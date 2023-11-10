using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Models.Event.Input;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface IEventRepository : IRepositoryBase<Event>
{
    Task<Event?> GetDetailBySorted(int sorted);
    Task<(int TotalCount, List<Event> Items)> GetPaginationAsync(PaginationEventInput input, Guid userId);
    void CancelAsync(Event entity);
    Task<List<Event>> GetListAsync(Expression<Func<Event, bool>> predicate);
}
