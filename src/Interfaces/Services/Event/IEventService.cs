using Donace_BE_Project.Enums.Entity;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Models.EventParticipation;
using Donace_BE_Project.Shared.Pagination;

namespace Donace_BE_Project.Interfaces.Services.Event;
public interface IEventService
{
    Task CancelAsync(Guid id);
    Task<EventFullOutput> CreateAsync(EventCreateInput input);
    Task<EventDetailModel> GetDetailBySortedAsync(int sorted, Guid calendarId);
    Task<PaginationOutput<EventOutput>> GetPaginationAsync(PaginationEventInput input);
    Task<EventDetailModel> GetDetailByIdAsync(Guid id);
    Task<ResponseModel<EventsModelResponse>> GetListEventByCalendarAsync();
    Task UpdateAsync(EventUpdateInput input);
    Task<List<EventOutput>> GetListEventInLocationAsync(string location);
    Task UserJoinEventAsync(UserJoinEventModel req);
    Task<List<EventFullOutput>> GetListEventByUserAsync(bool isNew);
    Task<List<EventFullOutput>> GetListEventByCalendarAsync(Guid id, bool isNew, bool isSub);
    Task<bool> ApprovalAsync(Guid idPart, EventParticipationStatus status, string qr);
    Task<List<EventParticipationApprovalModel>> ListUserJoinEventAsync(Guid eventId);
}