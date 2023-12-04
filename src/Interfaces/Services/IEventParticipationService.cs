using Donace_BE_Project.Enums.Entity;
using Donace_BE_Project.Models.EventParticipation;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IEventParticipationService
    {
        Task CreateAsync(EventParticipationModel req);
        Task<Dictionary<Guid, EventParticipationStatus>> ListIdEventSubAsync(Guid userId, bool isNew);

        /// <summary>
        /// Lấy Danh sách Event và status của calendar mà User join
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isNew"></param>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, EventParticipationStatus>> ListIdEventSubByCalendarAsync(Guid calendarId);

        /// <summary>
        /// Lấy tất cả Id của event mà user đã join
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<Guid, EventParticipationStatus>> GetAllIdEventUserJoinAsync(Guid userId);

        Task<EventParticipationStatus> StatusEventJoinAsync(Guid userId);

        Task<EventParticipationModel> GetByIdAsync(Guid id);
    }
}
