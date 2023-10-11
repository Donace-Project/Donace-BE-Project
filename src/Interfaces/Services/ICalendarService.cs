using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;

namespace Donace_BE_Project.Interfaces.Services;

public interface ICalendarService
{
    Task<ResponseModel<CalendarModel>> CreateAsync(CalendarModel model);
    Task<ResponseModel<CalendarUpdateModel>> UpdateAsync(CalendarUpdateModel model);
    Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id);
    Task<ResponseModel<GetListCalendarModel>> GetListCalendarAsync(RequestBaseModel input);
}
