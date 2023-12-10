using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
using Donace_BE_Project.Enums.Entity;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Donace_BE_Project.EntityFramework.Repositories
{
    public class EventParticipationRepository : RepositoryBase<EventParticipation>, IEventParticipationRepository
    {
        public EventParticipationRepository(CalendarDbContext db, IUserProvider userProvider) : base(db, userProvider)
        {
        }

        public async Task<Dictionary<Guid, EventParticipationStatus>> GetAllIdEventUserJoinAsync(Guid userId)
        {
            return await _dbSet.Where(x => x.CreatorId == userId && 
                                           x.IsDeleted == false && 
                                           x.Status != EventParticipationStatus.NotGoing).ToDictionaryAsync(x => x.EventId, a => a.Status);
        }

        public async Task<List<EventParticipation>> GetListByEventAsync(Guid eventId)
        {
            return await _dbSet.Where(x => x.EventId == eventId && x.IsDeleted == false && x.Status != EventParticipationStatus.NotGoing).ToListAsync();
        }

        public async Task<Dictionary<Guid, EventParticipationStatus>> ListIdEventByCalendarAsync(Guid calendarId)
        {
            return await _dbSet.Where(x => x.Event.CalendarId == calendarId).ToDictionaryAsync(x => x.EventId, a => a.Status);
        }

        public async Task<Dictionary<Guid, EventParticipationStatus>> ListIdEventByUserIdAsync(Guid userId, bool isNew)
        {
            return isNew ? await _dbSet.Where(x => x.UserId == userId
                                           && x.Event.EndDate > DateTime.Now).ToDictionaryAsync(x => x.EventId, x => x.Status)
                         : await _dbSet.Where(x => x.UserId == userId
                                           && x.Event.EndDate < DateTime.Now)
                                        .ToDictionaryAsync(x => x.EventId, x => x.Status);
        }
    }
}
