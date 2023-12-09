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

    /// <summary>
    /// API tạo calendar
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("create-calendar")]
    public async Task<ResponseModel<CalendarResponseModel>> CreateAsync(CalendarModel model)
    {
        return await _iCalendarService.CreateAsync(model);
    }

    /// <summary>
    /// API Update calendar
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("update-calendar")]
    public async Task<ResponseModel<CalendarResponseModel>> UpdateAsync(CalendarUpdateModel model)
    {
        return await _iCalendarService.UpdateAsync(model);
    }

    /// <summary>
    /// API xóa calendar
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [HttpDelete("delete-calendar")]
    public async Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id)
    {
        return await _iCalendarService.DeleteAsync(Id);
    }

    /// <summary>
    /// Lấy thông tin Calendar theo id
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    [HttpPost("get-by-id")]
    public async Task<CalendarResponseModel> GetAsync(Guid Id)
    {
        return await _iCalendarService.GetByIdAsync(Id);
    }

    /// <summary>
    /// API lấy danh sách calendar mà user tạo và join
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("get-list")]
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListAsync(RequestBaseModel input)
    {
        return await _iCalendarService.GetListCalendarAsync(input);
    }

    /// <summary>
    /// API lấy danh sách user trong calendar
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("get-list-user")]
    public async Task<ResponseModel<List<GetListUserInCalendarModel>>> GetListUserOfCalendarAsync(RequestGetListUserInCalendarModel input)
    {
        return await _iCalendarService.GetListUserInCalendarAsync(input);
    }

    /// <summary>
    /// API join calendar
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("user-join")]
    public async Task UserJoinCalendarAsync(UserJoinCalendarReqModel input)
    {
        await _iCalendarService.UserJoinCalendarAsync(input);
    }

    /// <summary>
    /// API lấy danh sách calendar mà user đã join
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost("get-list-subcribed")]
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListSubcribedAsync(RequestBaseModel input)
    {
        return await _iCalendarService.GetListCalendarSubcribedAsync(input);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost("invite-mail")]
    public async Task InviteSendMailJoinAsync(InviteJoinCalendarModel input)
    {
        await _iCalendarService.InviteJoinCalendarAsync(input);
    }
}
