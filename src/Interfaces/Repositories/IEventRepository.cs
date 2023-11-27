using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Models.Event.Input;
using System.Linq.Expressions;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface IEventRepository : IRepositoryBase<Event>
{
    Task<List<Event>> GetPaginationAsync(PaginationEventInput input, Guid userId);
    Task<Event?> GetDetailByIdAsync(Guid id);
    Task<Event?> GetDetailBySorted(int sorted);
    void CancelAsync(Event entity);
    Task<List<Event>> GetListAsync(Expression<Func<Event, bool>> predicate);
}
