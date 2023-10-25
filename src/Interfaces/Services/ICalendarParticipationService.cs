using Azure;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.CalendarParticipation;

namespace Donace_BE_Project.Interfaces.Services;

public interface ICalendarParticipationService
{
    Task<ResponseModel<CalendarParticipationModel>> CreateAsync(CalendarParticipationModel model, bool isSubcribed = false);
    Task<ResponseModel<CalendarParticipationGetByCalendarIdModel>> DeleteByCalendarUserIdAsync(CalendarParticipationGetBycalendarUserIdModel model);
    Task<ResponseModel<List<Guid>>> GetListIdCalendarByUserIdAsync();
    Task<ResponseModel<List<Guid>>> GetListUserIdOfCalendarAsync(Guid idCalendar, int pageNumber, int pageSize);
    Task<ResponseModel<long>> TotalUserInCalendarAsync(Guid idCalendar);
}
