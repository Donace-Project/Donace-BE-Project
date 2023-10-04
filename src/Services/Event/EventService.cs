using AutoMapper;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;
using EventEnitity = Donace_BE_Project.Entities.Calendar.Event;
namespace Donace_BE_Project.Services.Event;

public class EventService : IEventService
{
    private readonly IEventRepository _repoEvent;
    private readonly ISectionRepository _repoSection;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService(IEventRepository repoEvent,
                        ISectionRepository repoSection,
                        IUnitOfWork unitOfWork,
                        IMapper mapper)
    {
        _repoEvent = repoEvent;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _repoSection = repoSection;
    }

    public async Task<EventFullOutput> CreateAsync(EventCreateInput input)
    {
        var eventEntity = _mapper.Map<EventCreateInput, EventEnitity>(input);

        var createdEvent = await _repoEvent.CreateAsync(eventEntity);
        await _unitOfWork.SaveChangeAsync();

        return _mapper.Map<EventEnitity, EventFullOutput>(createdEvent);
    }

    public async Task<PaginationOutput<EventFullOutput>> GetPaginationAsync(PaginationEventInput input)
    {
        var output = await _repoEvent.GetPaginationAsync(input);

        return new()
        {
            TotalCount = output.TotalCount,
            Items = _mapper.Map<List<EventEnitity>, List<EventFullOutput>>(output.Items),
        };
    }

    public async Task UpdateAsync(EventUpdateInput input)
    {
        EventEnitity foundEvent = await FindEventAsync(input.Id);

        foundEvent = _mapper.Map(input, foundEvent);
        _repoEvent.Update(foundEvent);
        await _repoSection.OverrideSections(foundEvent.Id, _mapper.Map<List<Section>>(input.Sections));
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        var foundEvent = await FindEventAsync(id);

        _repoEvent.CancelAsync(foundEvent);
        await _repoSection.CancelSections(id);
        await _unitOfWork.SaveChangeAsync();
    }
    private async Task<EventEnitity> FindEventAsync(Guid id)
    {
        var foundEvent = await _repoEvent.GetByIdAsync(id);
        if (foundEvent is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có id: {id}");
        }

        return foundEvent;
    }
}
