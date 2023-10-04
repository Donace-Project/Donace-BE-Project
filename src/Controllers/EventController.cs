using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventController : ControllerBase, IEventService
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
    public Task<EventFullOutput> CreateAsync(EventCreateInput input)
    {
        return _service.CreateAsync(input);
    }

    [HttpGet()]
    public Task<PaginationOutput<EventFullOutput>> GetPaginationAsync([FromQuery] PaginationEventInput input)
    {
        return _service.GetPaginationAsync(input);
    }

    [HttpPut()]
    public Task UpdateAsync(EventUpdateInput input)
    {
        return _service.UpdateAsync(input);
    }
}
