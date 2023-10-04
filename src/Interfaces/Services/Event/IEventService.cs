using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;

namespace Donace_BE_Project.Interfaces.Services.Event;
public interface IEventService
{
    Task<EventFullOutput> CreateAsync(EventCreateInput input);
    Task<PaginationOutput<EventFullOutput>> GetPaginationAsync(PaginationEventInput input);
    Task UpdateAsync(EventUpdateInput input);
}