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
    public async Task<ResponseModel<CalendarResponseModel>> CreateAsync(CalendarModel model)
    {
        return await _iCalendarService.CreateAsync(model);
    }

    [HttpPost("update-calendar")]
    public async Task<ResponseModel<CalendarResponseModel>> UpdateAsync(CalendarUpdateModel model)
    {
        return await _iCalendarService.UpdateAsync(model);
    }

    [HttpDelete("delete-calendar")]
    public async Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id)
    {
        return await _iCalendarService.DeleteAsync(Id);
    }

    [HttpPost("get-list")]
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListAsync(RequestBaseModel input)
    {
        return await _iCalendarService.GetListCalendarAsync(input);
    }

    [HttpPost("get-list-user")]
    public async Task<ResponseModel<List<GetListUserInCalendarModel>>> GetListUserOfCalendarAsync(RequestGetListUserInCalendarModel input)
    {
        return await _iCalendarService.GetListUserInCalendarAsync(input);
    }

    [HttpPost("user-join")]
    public async Task UserJoinCalendarAsync(UserJoinCalendarReqModel input)
    {
        await _iCalendarService.UserJoinCalendarAsync(input);
    }

    [HttpPost("get-list-subcribed")]
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListSubcribedAsync(RequestBaseModel input)
    {
        return await _iCalendarService.GetListCalendarSubcribedAsync(input);
    }
}
