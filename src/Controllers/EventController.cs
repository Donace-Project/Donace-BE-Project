using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _service;

    public EventController(IEventService service)
    {
        _service = service;
    }

    [HttpPost("cancel")]
    public Task CancelAsync(Guid id)
    {
        return _service.CancelAsync(id);
    }

    [HttpPost()]
    public async Task<EventFullOutput> CreateAsync([FromBody] EventCreateInput input)
    {
        return await _service.CreateAsync(input);
    }

    [HttpGet("detail-by-sorted")]
    public async Task<EventFullOutput> GetDetailBySorted(int sorted, Guid calendarId)
    {
        return await _service.GetDetailBySortedAsync(sorted, calendarId);
    }

    [HttpGet("detail-by-id")]
    public async Task<EventFullOutput> GetDetailById(Guid id)
    {
        return await _service.GetDetailByIdAsync(id);
    }

    [HttpGet()]
    public async Task<PaginationOutput<EventOutput>> GetPaginationAsync([FromQuery] PaginationEventInput input)
    {
        return await _service.GetPaginationAsync(input);
    }

    [HttpPut()]
    public Task UpdateAsync(EventUpdateInput input)
    {
        return _service.UpdateAsync(input);
    }

    [HttpGet("in")]
    public async Task<List<EventOutput>> GetListEventInLocationAsync(string location)
    {
        return await _service.GetListEventInLocationAsync(location);
    }

    [HttpPost("user-join")]
    public async Task UserJoinEventAsync(UserJoinEventModel req)
    {
        await _service.UserJoinEventAsync(req);
    }

    [HttpGet("list-event-by-user")]
    public async Task<List<EventFullOutput>> ListEventByUserAsync(bool IsNew = true)
    {
        return await _service.GetListEventByUserAsync(IsNew);
    }

    [HttpGet("list-event-by-calendar-{id}-{isNew}-{isSub}")]
    public async Task<List<EventFullOutput>> ListEventByCalendarAsync(Guid id, bool isNew = true, bool isSub = true)
    {
        return await _service.GetListEventByCalendarAsync(id, isNew, isSub);
    }
}
