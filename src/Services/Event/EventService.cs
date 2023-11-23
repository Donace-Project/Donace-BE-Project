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
using EventEntity = Donace_BE_Project.Entities.Calendar.Event;
namespace Donace_BE_Project.Services.Event;

public class EventService : IEventService
{
    private readonly IEventRepository _repoEvent;
    private readonly ISectionRepository _repoSection;
    private readonly ICalendarRepository _repoCalendar;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobClient _iBackgroundJobClient;
    private readonly ILogger<EventService> _iLogger;
    private readonly IUserProvider _iUserProvider;
    private readonly ICommonService _commonService;
    private readonly ILocationService _locationService;
    public EventService(IEventRepository repoEvent,
                        ISectionRepository repoSection,
                        ICalendarRepository repoCalendar,
                        IUnitOfWork unitOfWork,
                        IMapper mapper,
                        ICacheService cacheService,
                        IBackgroundJobClient backgroundJobClient,
                        ILogger<EventService> logger,
                        IUserProvider userProvider,
                        ICommonService commonService,
                        ILocationService locationService)
    {
        _repoEvent = repoEvent;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _repoSection = repoSection;
        _repoCalendar = repoCalendar;
        _iBackgroundJobClient = backgroundJobClient;
        _iLogger = logger;
        _iUserProvider = userProvider;
        _commonService = commonService;
        _locationService = locationService;
    }

    /// <summary>
    /// Tạo Event
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<EventFullOutput> CreateAsync(EventCreateInput input)
    {
        try
        {
            await IsValidCalendar(input.CalendarId);

            var eventEntity = _mapper.Map<EventCreateInput, EventEntity>(input);
            eventEntity.LocationCode = input.Long != null && input.Lat != null ?
                                       _locationService.GetAreaAsync((double)input.Long, (double)input.Lat) :
                                       "VietNam";

            var createdEvent = await _repoEvent.CreateAsync(eventEntity);
            await _unitOfWork.SaveChangeAsync();

            var result = _mapper.Map<EventEntity, EventFullOutput>(createdEvent);

            return result;
        }   
        catch (Exception ex)
        {
            throw new FriendlyException("400",ex.Message);
        }
    }

    /// <summary>
    /// Lấy data có paging
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PaginationOutput<EventOutput>> GetPaginationAsync(PaginationEventInput input)
    {
        var userId = _iUserProvider.GetUserId();

        var output = await _repoEvent.GetPaginationAsync(input, userId);

        return new()
        {
            TotalCount = output.TotalCount,
            Items = _mapper.Map<List<EventEntity>, List<EventOutput>>(output.Items),
        };
    }

    /// <summary>
    /// Lấy thông tin event theo sorted
    /// </summary>
    /// <param name="sorted"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<EventFullOutput> GetDetailBySortedAsync(int sorted, Guid calendarId)
    {
        var output = await _repoEvent.GetDetailBySorted(sorted);

        if (output is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có sort: {sorted}");
        }

        return _mapper.Map<EventEntity, EventFullOutput>(output);
    }

    public async Task<EventFullOutput> GetDetailByIdAsync(Guid id)
    {
        var output = await _repoEvent.GetByIdAsync(id);

        if (output is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event");
        }

        return _mapper.Map<EventEntity, EventFullOutput>(output);
    }

    /// <summary>
    /// Update event
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateAsync(EventUpdateInput input)
    {
        await IsValidCalendar(input.CalendarId);
        EventEntity foundEvent = await FindEventAsync(input.Id);

        foundEvent = _mapper.Map(input, foundEvent);
        _repoEvent.Update(foundEvent);
        await _repoSection.OverrideSections(foundEvent.Id, _mapper.Map<List<Section>>(input.Sections));
        await _unitOfWork.SaveChangeAsync();
    }

    /// <summary>
    /// Cancel event
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
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

    /// <summary>
    /// Lấy Danh sách Event tại 3 thành Phố lớn: Hà Nội, đà nẵng, HCM
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<List<EventOutput>> GetListEventInLocationAsync(string location)
    {
        try
        {
            var events = await _repoEvent.GetListAsync(x => x.LocationCode == location);

            return _mapper.Map<List<EventOutput>>(events);
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }
}
