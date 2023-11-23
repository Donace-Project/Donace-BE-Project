using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.EntityFramework.Db;
using Donace_BE_Project.EntityFramework.Repository.Base;
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

        public async Task<List<Guid>> ListIdEventByUserIdAsync(Guid userId)
        {
            return await _dbSet.Where(x => x.UserId == userId).Select(x => x.EventId).ToListAsync();
        }
    }
}
