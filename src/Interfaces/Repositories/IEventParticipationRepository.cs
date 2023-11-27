using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.Enums.Entity;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IEventParticipationRepository : IRepositoryBase<EventParticipation>
    {
        Task<Dictionary<Guid, EventParticipationStatus>> ListIdEventByUserIdAsync(Guid userId, bool isNew);

        /// <summary>
        /// Lấy List Event id và status của event trong calendar
        /// </summary>
        /// <param name="calendarId"></param>
        /// <returns></returns>
        Task<Dictionary<Guid, EventParticipationStatus>> ListIdEventByCalendarAsync(Guid calendarId);
    }
}
