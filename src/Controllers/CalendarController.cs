using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _iCalendarService;
    public CalendarController(ICalendarService calendar)
    {
        _iCalendarService = calendar;
    }

    [HttpPost("create-calendar")]
    public async Task<ResponseModel<CalendarModel>> CreateAsync(CalendarModel model)
    {
        return await _iCalendarService.CreateAsync(model);
    }

    [HttpPost("update-calendar")]
    public async Task<ResponseModel<CalendarUpdateModel>> UpdateAsync(CalendarUpdateModel model)
    {
        return await _iCalendarService.UpdateAsync(model);
    }

    [HttpDelete("delete-calendar")]
    public async Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id)
    {
        return await _iCalendarService.DeleteAsync(Id);
    }
}
