using AutoMapper;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService(IEventRepository repoEvent, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repoEvent = repoEvent;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
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
}
