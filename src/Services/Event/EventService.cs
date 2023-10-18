using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.EntityFramework.Repositories;
using Donace_BE_Project.EntityFramework;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models.CalendarParticipation;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;
using Newtonsoft.Json;
using EventEntity = Donace_BE_Project.Entities.Calendar.Event;
namespace Donace_BE_Project.Services.Event;

public class EventService : IEventService
{
    private readonly IEventRepository _repoEvent;
    private readonly ISectionRepository _repoSection;
    private readonly ILogger<CalendarParticipationService> _iLogger;
    private readonly ICalendarRepository _repoCalendar;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EventService(IEventRepository repoEvent,
                        ISectionRepository repoSection,
                        ICalendarRepository repoCalendar,
                         ILogger<CalendarParticipationService> logger,
                        IUnitOfWork unitOfWork,
                        IMapper mapper)
    {
        _repoEvent = repoEvent;
        _unitOfWork = unitOfWork;
        _iLogger = logger;
        _mapper = mapper;
        _repoSection = repoSection;
        _repoCalendar = repoCalendar;
    }

    public async Task<EventFullOutput> CreateAsync(EventCreateInput input)
    {
        await IsValidCalendar(input.CalendarId);

        var eventEntity = _mapper.Map<EventCreateInput, EventEntity>(input);

        var createdEvent = await _repoEvent.CreateAsync(eventEntity);
        await _unitOfWork.SaveChangeAsync();

        return _mapper.Map<EventEntity, EventFullOutput>(createdEvent);
    }

    public async Task<PaginationOutput<EventOutput>> GetPaginationAsync(PaginationEventInput input)
    {
        var output = await _repoEvent.GetPaginationAsync(input);

        return new()
        {
            TotalCount = output.TotalCount,
            Items = _mapper.Map<List<EventEntity>, List<EventOutput>>(output.Items),
        };
    }

    public async Task<EventFullOutput> GetDetailById(Guid id)
    {
        var output = await _repoEvent.GetDetailById(id);
        if (output is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có id: {id}");
        }

        return _mapper.Map<EventEntity, EventFullOutput>(output);
    }

    public async Task UpdateAsync(EventUpdateInput input)
    {
        await IsValidCalendar(input.CalendarId);
        EventEntity foundEvent = await FindEventAsync(input.Id);

        foundEvent = _mapper.Map(input, foundEvent);
        _repoEvent.Update(foundEvent);
        await _repoSection.OverrideSections(foundEvent.Id, _mapper.Map<List<Section>>(input.Sections));
        await _unitOfWork.SaveChangeAsync();
    }

    public async Task CancelAsync(Guid id)
    {
        var foundEvent = await FindEventAsync(id);
        if(foundEvent.IsEnable == false)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có id: {id}");
        }

        _repoEvent.CancelAsync(foundEvent);
        await _repoSection.CancelSections(id);
        await _unitOfWork.SaveChangeAsync();
    }
    private async Task<EventEntity> FindEventAsync(Guid id)
    {
        var foundEvent = await _repoEvent.GetByIdAsync(id);
        if (foundEvent is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có id: {id}");
        }

        return foundEvent;
    }

    private async Task IsValidCalendar(Guid calendarId)
    {
        var calendar = await _repoCalendar.GetByIdAsync(calendarId);
        if(calendar is null)
        {
            throw new FriendlyException(string.Empty, "Không tìm thấy calendar");
        }
    }
    public async Task<ResponseModel<EventCreateInput>> CreateAsyncEvent(EventCreateInput model)
    {
        try
        {
            var calendarParti = _mapper.Map<EventCreateInput>(model);

            await _repoEvent.CreateAsync(calendarParti);
            await _unitOfWork.SaveChangeAsync();
            return new ResponseModel<EventCreateInput>(true, ResponseCode.Donace_BE_Project_Bad_Request_EventService_Success, model, new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }
}
