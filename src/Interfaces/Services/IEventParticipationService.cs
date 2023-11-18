using Donace_BE_Project.Models.EventParticipation;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IEventParticipationService
    {
        Task CreateAsync(EventParticipationModel req);
        Task<List<Guid>> ListIdEventSubAsync(Guid userId);
    }
}
