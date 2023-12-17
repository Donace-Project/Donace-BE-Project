using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Entities.Event;
using Donace_BE_Project.Entities.Ticket;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Enums.Entity;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.CalendarParticipation;
using Donace_BE_Project.Models.Event.Input;
using Donace_BE_Project.Models.Event.Output;
using Donace_BE_Project.Models.EventParticipation;
using Donace_BE_Project.Shared.Pagination;
using Hangfire;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Nest;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
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
    private readonly ICalendarParticipationService _calendarParticipationService;
    private readonly IEventParticipationService _eventParticipationService;
    private readonly IEventParticipationRepository _eventParticipationRepository;
    private readonly IUserService _userService;
    private readonly ITicketsRepository _ticketsRepository;
    private readonly IUserTicketsRepository _userTicketsRepository;
    private readonly ICalendarParticipationRepository _calendarParticipationRepository;
    private readonly ICacheService _cacheService;
    private readonly IUserRepository _iUserRepository;
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
                        ILocationService locationService,
                        ICalendarParticipationService calendarParticipationService,
                        IEventParticipationService eventParticipationService,
                        IEventParticipationRepository eventParticipationRepository,
                        IUserService userService,
                        ITicketsRepository ticketsRepository,
                        IUserTicketsRepository userTicketsRepository,
                        ICalendarParticipationRepository calendarParticipationRepository,
                        IUserRepository iUserRepository)
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
        _calendarParticipationService = calendarParticipationService;
        _eventParticipationService = eventParticipationService;
        _eventParticipationRepository = eventParticipationRepository;
        _userService = userService;
        _ticketsRepository = ticketsRepository;
        _userTicketsRepository = userTicketsRepository;
        _calendarParticipationRepository = calendarParticipationRepository;
        _cacheService = cacheService;
        _iUserRepository = iUserRepository;
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

            var ticket = _mapper.Map<Ticket>(input.Ticket);

            var createdEvent = await _repoEvent.CreateAsync(eventEntity);

            ticket.EventId = createdEvent.Id;

            await _ticketsRepository.CreateAsync(ticket);

            await _unitOfWork.SaveChangeAsync();

            var result = _mapper.Map<EventEntity, EventFullOutput>(createdEvent);


            return result;
        }   
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
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

        // List Event của user tạo
        var output = await _repoEvent.GetPaginationAsync(input, userId);

        // List EventId của user Join
        var listIdEventSubs = input.IsNew == null ?
                                await _eventParticipationService.GetAllIdEventUserJoinAsync(userId) :
                                await _eventParticipationService.ListIdEventSubAsync(userId, input.IsNew.Value);

        var result = _mapper.Map<List<EventEntity>, List<EventOutput>>(output.OrderByDescending(x => x.StartDate).ToList());
        if (!listIdEventSubs.Any())
        {
            return new()
            {
                TotalCount = 0,
                Items = result,
            };
        }

        var listId = listIdEventSubs.Keys.ToList();
        var listEventSubs = await _repoEvent.GetListAsync(x => listId.Contains(x.Id));
        var resultSub = _mapper.Map<List<EventOutput>>(listEventSubs);

        foreach(var item in resultSub)
        {
            item.IsHost = false;

            var statusEnum = listIdEventSubs.Where(x => x.Key == item.Id).Select(x => x.Value).First();

            item.Status = statusEnum == EventParticipationStatus.Approval ? "Chờ Xác Nhận"
                           : statusEnum == EventParticipationStatus.NotGoing ? "Không Tham Gia"
                           : statusEnum == EventParticipationStatus.Going ? "Đã Tham Gia"
                           : "Đã CheckIn";
        }

        result.AddRange(resultSub);
        return new()
        {
            TotalCount = 0,
            Items = result.OrderByDescending(x => x.StartDate).ToList(),
        };
    }

    /// <summary>
    /// Lấy thông tin event theo sorted
    /// </summary>
    /// <param name="sorted"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<EventDetailModel> GetDetailBySortedAsync(int sorted, Guid calendarId)
    {
        var output = await _repoEvent.GetDetailBySorted(sorted);        

        if (output is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có sort: {sorted}");
        }

        var result = _mapper.Map<EventEntity, EventDetailModel>(output);
        var userId = _iUserProvider.GetUserId();

        if (userId == output.CreatorId)
        {
            result.IsHost = true;
            return result;
        }

        var check  = await _eventParticipationService.StatusEventJoinAsync(userId);
        result.IsFree = (await _ticketsRepository.FindAsync(x => x.EventId == output.Id)).IsFree;

        switch (check)
        {
            case EventParticipationStatus.NotGoing:
                result.IsSub = false;
                result.IsAppro = false;
                return result;

            case EventParticipationStatus.Approval:
                result.IsSub = false;
                result.IsAppro = true;
                return result;
        }

        await _ticketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                    x.EventId == userId);

        result.IsCheckAppro = (await _ticketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                                      x.EventId == output.Id)).IsRequireApprove;
        return result;
    }

    public async Task<EventDetailModel> GetDetailByIdAsync(Guid id)
    {
        var output = await _repoEvent.GetByIdAsync(id);

        if (output is null)
        {
            throw new FriendlyException(string.Empty, $"Không tìm thấy event có sort: {id}");
        }

        var result = _mapper.Map<EventEntity, EventDetailModel>(output);
        var userId = _iUserProvider.GetUserId();
        var ticket = await _ticketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                                x.EventId == id);
        var userHost = await _iUserRepository.FindByIdAsync(ticket.CreatorId);
        result.IsFree = ticket != null ? ticket.IsFree : false;
        result.IsCheckAppro = ticket != null ? ticket.IsRequireApprove : false;
        result.Price = ticket != null ? ticket.Price : 0;
        result.Email = userHost != null ? userHost.Email : null;
        result.TicketId = ticket != null ? ticket.Id : null;

        if (userId == output.CreatorId)
        {
            result.IsHost = true;
            return result;
        }
        else
        {
            result.IsHost = false;
        }

        var check = await _eventParticipationService.StatusEventJoinAsync(userId);

        switch (check)
        {
            case EventParticipationStatus.NotGoing:
                result.IsSub = false;
                result.IsAppro = false;
                return result;

            case EventParticipationStatus.Approval:
                result.IsSub = false;
                result.IsAppro = true;
                return result;
        }        
        return result;
    }

    /// <summary>
    /// Update event
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateAsync(EventUpdateInput input)
    {
        try
        {
            await IsValidCalendar(input.CalendarId);
            EventEntity foundEvent = await FindEventAsync(input.Id);

            foundEvent = _mapper.Map(input, foundEvent);
            _repoEvent.Update(foundEvent);
            await _unitOfWork.SaveChangeAsync();

        }
        catch(FriendlyException ex)
        {
            throw new FriendlyException("", ex.Message);
        }
    }

    public async Task UpdateCoverAsync(EventForUpdateCover input)
    {
        try
        {
            EventEntity foundEvent = await FindEventAsync(input.Id);
            foundEvent.Cover = input.Cover;
            _repoEvent.Update(foundEvent);
            await _unitOfWork.SaveChangeAsync();
        }
        catch (FriendlyException ex)
        {
            throw new FriendlyException("", ex.Message);
        }
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

    public async Task UserJoinEventAsync(UserJoinEventModel req)
    {
        try
        {
            var events = await _repoEvent.GetByIdAsync(req.EventId);
            var userId = _iUserProvider.GetUserId();

            if(events is null)
            {
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_EventService, "không tìm thấy Event");
            }

            if(events.Capacity == 0 && !events.IsUnlimited)
            {
                throw new FriendlyException("400", "Đã hết vé");
            }

            events.Capacity--;
            _repoEvent.Update(events);

            var checkPartCalendar = await _calendarParticipationRepository.FindAsync(x => x.IsDeleted == false &&
                                                                                          x.UserId == userId);
            if (checkPartCalendar is null)
            {
                await _calendarParticipationRepository.CreateAsync(new CalendarParticipation
                {
                    CalendarId = events.CalendarId,
                    UserId = userId,
                    IsSubcribed = true,
                });

                // Update cache
                                
                var calendar = await _repoCalendar.FindAsync(x => x.Id == events.CalendarId &&
                                                                  x.IsDeleted == false);

                var listIdJoin = (await _calendarParticipationRepository.ToListAsync(x => x.IsDeleted == false &&
                                                                                          x.CalendarId == events.CalendarId &&
                                                                                          x.IsSubcribed == true))
                                                                                          .Select(x => x.UserId).ToList();
                var dataCache = _mapper.Map<CalendarResponseModel>(calendar);

                dataCache.IsHost = false;
                dataCache.IsSub = true;
                dataCache.TotalSubscriber += 1;
                foreach(var id in listIdJoin)
                {
                    await _cacheService.RemoveItemDataBySortedAsync($"{KeyCache.Calendar}:{id}", calendar.Sorted);
                    await _cacheService.UpdateValueScoreAsync($"{KeyCache.Calendar}:{id}", calendar.Sorted, dataCache);
                }

                dataCache.IsHost = true;
                dataCache.IsSub = false;
                await _cacheService.RemoveItemDataBySortedAsync($"{KeyCache.Calendar}:{calendar.CreatorId}", calendar.Sorted);
                await _cacheService.UpdateValueScoreAsync($"{KeyCache.Calendar}:{calendar.CreatorId}", calendar.Sorted, dataCache);
            }

            var checkPart = await _eventParticipationRepository.FindAsync(x => x.IsDeleted == false
                                                                            && x.CreatorId == userId
                                                                            && x.EventId == events.Id);

            if(checkPart is null)
            {
                await _eventParticipationService.CreateAsync(new EventParticipationModel
                {
                    UserId = _iUserProvider.GetUserId(),
                    EventId = events.Id,
                    Status = EventParticipationStatus.Approval
                });

                await _unitOfWork.SaveChangeAsync();
                return;
            }

            if(checkPart.Status == EventParticipationStatus.Approval)
            {
                throw new FriendlyException("400", "user này đã yêu cầu join");
            }

            if(checkPart.Status != EventParticipationStatus.NotGoing)
            {
                throw new FriendlyException("400", "User này đã join vào event");
            }

            checkPart.Status = EventParticipationStatus.Approval;
            _eventParticipationRepository.Update(checkPart);

            
            await _unitOfWork.SaveChangeAsync();
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }

    public async Task<List<EventFullOutput>> GetListEventByUserAsync(bool isNew)
    {
        try
        {
            var idUser = _iUserProvider.GetUserId();
            var myEvent = isNew ? await _repoEvent.GetListAsync(x => x.CreatorId == idUser && x.EndDate >= DateTime.Now)
                                : await _repoEvent.GetListAsync(x => x.CreatorId == idUser && x.EndDate <= DateTime.Now);
            var subEventIds = await _eventParticipationService.ListIdEventSubAsync(idUser, isNew);

            if (!subEventIds.Any())
            {
                return _mapper.Map<List<EventFullOutput>>(myEvent.OrderByDescending(x => x.StartDate));
            }

            var listIdSub = subEventIds.Keys.ToList();
            var subEvents = await _repoEvent.GetListAsync(x => listIdSub.Contains(x.Id));

            var dataMy = _mapper.Map<List<EventFullOutput>>(myEvent);
            var dataSub = _mapper.Map<List<EventFullOutput>>(subEvents);

            foreach( var item in dataSub )
            {
                item.IsHost = false;

                var statusEnum = subEventIds.Where(x => x.Key == item.Id).Select(x => x.Value).First();

                item.Status = statusEnum == EventParticipationStatus.Approval ? "Chờ Xác Nhận"
                           : statusEnum == EventParticipationStatus.NotGoing ? "Không Tham Gia"
                           : statusEnum == EventParticipationStatus.Going ? "Đã Tham Gia"
                           : "Đã CheckIn";
            }


            dataMy.AddRange(dataSub);

            return _mapper.Map<List<EventFullOutput>>(myEvent.OrderByDescending(x => x.StartDate));
        }
        catch (FriendlyException ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}", _iUserProvider.GetUserId());
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }

    public async Task<List<EventFullOutput>> GetListEventByCalendarAsync(Guid id, bool isNew, bool isSub)
    {
        try
        {            
            if (!isSub)
            {
                var events = isNew ? await _repoEvent.GetListAsync(x => x.CalendarId.Equals(id) && x.EndDate >= DateTime.Now) :
                                 await _repoEvent.GetListAsync(x => x.CalendarId.Equals(id) && x.EndDate < DateTime.Now);

                return events.Any() ? _mapper.Map<List<EventFullOutput>>(events) : new List<EventFullOutput>();
            }

            var listIdEventStatus = await _eventParticipationService.ListIdEventSubByCalendarAsync(id);

            if (!listIdEventStatus.Any())
            {
                return new List<EventFullOutput>();
            }

            var listIdPart = listIdEventStatus.Keys.ToList();
            var listEventSubs = await _repoEvent.GetListAsync(x => listIdPart.Contains(x.Id));
            var resultSubs = _mapper.Map<List<EventFullOutput>>(listEventSubs);

            foreach(var item in resultSubs)
            {
                item.IsHost = false;

                var statusEnum = listIdEventStatus.Where(x => x.Key == item.Id).Select(x => x.Value).First();

                item.Status = statusEnum == EventParticipationStatus.Approval ? "Chờ Xác Nhận"
                           : statusEnum == EventParticipationStatus.NotGoing ? "Không Tham Gia"
                           : statusEnum == EventParticipationStatus.Going ? "Đã Tham Gia"
                           : "Đã CheckIn";
            }
            return resultSubs;
        }
        catch (FriendlyException ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}", _iUserProvider.GetUserId());
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }

    /// <summary>
    /// Approval User yêu cầu join vào event
    /// </summary>
    /// <param name="idPart"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<bool> ApprovalAsync(ApprovalEventInput input)
    {
        try
        {
            var eventPart = await _eventParticipationRepository.GetByIdAsync(input.IdPart);

            if (eventPart is null)
            {
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, "User này không có trong event");
            }

            var userId = _iUserProvider.GetUserId();

            var events = await _repoEvent.FindAsync(x => x.IsDeleted == false && x.Id == eventPart.EventId);

            if (events is null)
            {
                throw new FriendlyException("404", "Event không tồn tại");
            }

            if(userId != events.CreatorId)
            {
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_EventService, "Bạn không có quyền approval");
            }

            if (input.Status == EventParticipationStatus.NotGoing)
            {
                events.Capacity++;
                _repoEvent.Update(events);
                await _unitOfWork.SaveChangeAsync();
            }

            if(input.Status == EventParticipationStatus.Going)
            {
                events.TotalGuest++;
                _repoEvent.Update(events);
                await _unitOfWork.SaveChangeAsync();
            }

            eventPart.Status = input.Status;

            _eventParticipationRepository.Update(eventPart);
            if(input.Status == EventParticipationStatus.Going)
            {
                var ticket = await _ticketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                                     x.EventId == events.Id);
                
                if (ticket is null)
                {
                    throw new FriendlyException("404", "Event này chưa tạo ticket");
                }

                await _userTicketsRepository.CreateAsync(new UserTicket
                {
                    QrCode = input.Qr,
                    IsChecked = false,
                    UserId = input.UserId,
                    TicketId = ticket.Id,
                });
            }           

            await _unitOfWork.SaveChangeAsync();
            return true;
        }
        catch(FriendlyException ex)
        {
            await _unitOfWork.SaveChangeAsync();
            _iLogger.LogError($"EventService.Exception: {ex.Message}", JsonConvert.SerializeObject( new { input.IdPart, input.Status }));
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }

    /// <summary>
    /// Lấy danh sách User join event
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<List<EventParticipationApprovalModel>> ListUserJoinEventAsync(Guid eventId)
    {
        try
        {
            var eventParts = await _eventParticipationRepository.GetListByEventAsync(eventId);

            if (!eventParts.Any())
            {
                return new List<EventParticipationApprovalModel>();
            }

            var result = _mapper.Map<List<EventParticipationApprovalModel>>(eventParts.OrderByDescending(x => x.CreationTime));
            var listUserId = eventParts.Select(x => x.UserId).ToList();
            var listUser = await _userService.ListUserAsync(listUserId);

            foreach(var item in result)
            {
                var user = listUser?.Result.FirstOrDefault(x => x.Id == item.UserId);
                item.Name = user?.UserName;
                item.Avatar = user?.Avatar;
                item.Email = user?.Email;
            }

            return result;
        }
        catch(FriendlyException ex)
        {
            _iLogger.LogError($"EventService.Exception: {ex.Message}", eventId);
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_EventService, ex.Message);
        }
    }


    #region Private method

    public async Task AutoSendMailForUserEventAsync(Guid id)
    {
        try
        {
            var eventPart = await _eventParticipationRepository.ToListAsync(x => x.IsDeleted == false &&
                                                                                 x.EventId == id &&
                                                                                 x.Status == EventParticipationStatus.Going);
            var events = await _repoEvent.FindAsync(x => x.IsDeleted == false &&
                                                         x.Id == id);

            var listId = eventPart.Select(x => x.CreatorId).ToList();
            listId.Add(events.CreatorId);

        }
        catch(FriendlyException ex)
        {
            throw new FriendlyException("400", ex.Message);
        }
    }

    #endregion
}
