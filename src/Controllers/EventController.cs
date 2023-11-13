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

    [HttpGet("detail")]
    public async Task<EventFullOutput> GetDetailById(int sorted, Guid calendarId)
    {
        return await _service.GetDetailBySortedAsync(sorted, calendarId);
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
}
