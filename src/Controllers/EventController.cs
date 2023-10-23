﻿using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

//[Authorize]
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
    public Task<EventFullOutput> CreateAsync([FromForm]EventCreateInput input)
    {
        return _service.CreateAsync(input);
    }

    [HttpGet("detail")]
    public async Task<EventFullOutput> GetDetailById(int sorted, Guid calendarId)
    {
        return await _service.GetDetailBySortedAsync(sorted, calendarId);
    }

    [HttpGet()]
    public Task<PaginationOutput<EventOutput>> GetPaginationAsync([FromQuery] PaginationEventInput input)
    {
        return _service.GetPaginationAsync(input);
    }

    [HttpPut()]
    public Task UpdateAsync(EventUpdateInput input)
    {
        return _service.UpdateAsync(input);
    }
}
