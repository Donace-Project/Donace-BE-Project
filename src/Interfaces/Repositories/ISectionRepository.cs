using Donace_BE_Project.Entities.Calendar;

namespace Donace_BE_Project.Interfaces.Repositories;

public interface ISectionRepository : IRepositoryBase<Section>
{
    Task CancelSections(Guid eventId);
    Task OverrideSections(Guid eventId, List<Section> entities);
}
