using Azure;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.CalendarParticipation;

namespace Donace_BE_Project.Interfaces.Services;

public interface ICalendarParticipationService
{
    Task<ResponseModel<CalendarParticipationModel>> CreateAsync(CalendarParticipationModel model, bool isSubcribed = false);
    Task<ResponseModel<CalendarParticipationGetByCalendarIdModel>> DeleteByCalendarUserIdAsync(CalendarParticipationGetBycalendarUserIdModel model);

    /// <summary>
    /// Lấy danh sách calendar mà User đã tạo
    /// </summary>
    /// <returns></returns>
    Task<ResponseModel<List<Guid>>> GetListIdCalendarByUserIdAsync();
    Task<ResponseModel<List<Guid>>> GetListUserIdOfCalendarAsync(Guid idCalendar, int pageNumber, int pageSize);
    Task<ResponseModel<long>> TotalUserInCalendarAsync(Guid idCalendar);

    /// <summary>
    /// Lấy danh sách id calendar là user đã join
    /// </summary>
    /// <returns></returns>
    Task<List<Guid>> GetListIdCalendarUserJoinAsync();
}
