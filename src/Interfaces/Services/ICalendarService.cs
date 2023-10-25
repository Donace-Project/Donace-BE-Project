using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;

namespace Donace_BE_Project.Interfaces.Services;

public interface ICalendarService
{
    Task<ResponseModel<CalendarResponseModel>> CreateAsync(CalendarModel model);
    Task<ResponseModel<CalendarResponseModel>> UpdateAsync(CalendarUpdateModel model);
    Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id);
    Task<ResponseModel<List<GetListCalendarModel>>> GetListCalendarAsync(RequestBaseModel input);
    Task<ResponseModel<List<GetListUserInCalendarModel>>> GetListUserInCalendarAsync(RequestGetListUserInCalendarModel input);
    Task UserJoinCalendarAsync(UserJoinCalendarReqModel input);
}
