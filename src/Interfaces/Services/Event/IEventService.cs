using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;

namespace Donace_BE_Project.Interfaces.Services.Event;
public interface IEventService
{
    Task CancelAsync(Guid id);
    Task<EventFullOutput> CreateAsync(EventCreateInput input);
    Task<EventFullOutput> GetDetailBySortedAsync(int sorted, Guid calendarId);
    Task<PaginationOutput<EventOutput>> GetPaginationAsync(PaginationEventInput input);
    Task UpdateAsync(EventUpdateInput input);
    Task<ResponseModel<EventsModelResponse>> GetListEventByCalendarAsync();
    Task<List<EventOutput>> GetListEventInLocationAsync(string location);
    Task UserJoinEventAsync(UserJoinEventModel req);
    Task<List<EventFullOutput>> GetListEventByUserAsync();
    Task<List<EventFullOutput>> GetListEventByCalendarAsync(Guid id);
}