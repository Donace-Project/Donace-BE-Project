using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Shared.Pagination;
using Hangfire;
using Newtonsoft.Json;
using EventEntity = Donace_BE_Project.Entities.Calendar.Event;
namespace Donace_BE_Project.Services.Event;

public class EventService : IEventService
{
    private readonly IEventRepository _repoEvent;
    private readonly ISectionRepository _repoSection;
    private readonly ICalendarRepository _repoCalendar;
    private readonly ICacheService _iCacheService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobClient _iBackgroundJobClient;
    private readonly ILogger<EventService> _iLogger;
    public EventService(IEventRepository repoEvent,
                        ISectionRepository repoSection,
                        ICalendarRepository repoCalendar,
                        IUnitOfWork unitOfWork,
                        IMapper mapper,
                        ICacheService cacheService,
                        IBackgroundJobClient backgroundJobClient,
                        ILogger<EventService> logger)
    {
        _repoEvent = repoEvent;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _repoSection = repoSection;
        _repoCalendar = repoCalendar;
        _iCacheService = cacheService;
        _iBackgroundJobClient = backgroundJobClient;
        _iLogger = logger;
    }

    public async Task<EventFullOutput> CreateAsync(EventCreateInput input)
    {
        try
        {
            var listStrCache = new List<string>();
            await IsValidCalendar(input.CalendarId);

            var eventEntity = _mapper.Map<EventCreateInput, EventEntity>(input);

            var createdEvent = await _repoEvent.CreateAsync(eventEntity);
            await _unitOfWork.SaveChangeAsync();

            var dataCache = await _iCacheService.GetDataByKeyAsync<List<string>>(KeyCache.CacheSuggestLocation);
            if (dataCache.Result != null)
            {
                listStrCache.AddRange(dataCache.Result);
            }

            listStrCache.Add(input.AddressName);
            await _iCacheService.SetDataAsync(KeyCache.CacheSuggestLocation, listStrCache);

            var result = _mapper.Map<EventEntity, EventFullOutput>(createdEvent);
            await _iCacheService.SetListDataSortedAsync($"{KeyCache.CacheEvent}:{result.CalendarId}",result);

            return result;
        }   
        catch (Exception ex)
        {
            throw new FriendlyException("400",ex.Message);
        }
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

        await Task.WhenAll(_iCacheService.RemoveItemDataBySortedAsync($"{KeyCache.CacheEvent}:{foundEvent.CalendarId}", foundEvent.Sorted),
                           _repoSection.CancelSections(id));
        
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
    public async Task<ResponseModel<EventsModelResponse>> GetListEventByCalendarAsync()
    {
        try
        {
            throw new FriendlyException();
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }
}
