using Donace_BE_Project.Entities.Event;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IEventParticipationRepository : IRepositoryBase<EventParticipation>
    {
        Task<List<Guid>> ListIdEventByUserIdAsync(Guid userId);
    }
}
