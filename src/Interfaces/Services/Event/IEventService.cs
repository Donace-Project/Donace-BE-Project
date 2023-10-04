using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;

namespace Donace_BE_Project.Interfaces.Services.Event;
public interface IEventService
{
    Task CancelAsync(Guid id);
    Task<EventFullOutput> CreateAsync(EventCreateInput input);
    Task<EventFullOutput> GetDetailById(Guid id);
    Task<PaginationOutput<EventOutput>> GetPaginationAsync(PaginationEventInput input);
    Task UpdateAsync(EventUpdateInput input);
}