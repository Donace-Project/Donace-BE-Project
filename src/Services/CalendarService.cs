using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.CalendarParticipation;
using Nest;
using Newtonsoft.Json;
using OpenQA.Selenium.DevTools;
using System.Net.WebSockets;
using Calendar = Donace_BE_Project.Entities.Calendar.Calendar;

namespace Donace_BE_Project.Services;

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _iCalendarRepository;
    private readonly ICalendarParticipationService _iCalendarParticipationService;
    private readonly ILogger<CalendarService> _iLogger;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IUserProvider _userProvider;
    private readonly IUserService _iUserService;
    private readonly ICalendarParticipationRepository _calendarParticipationRepository;
    private readonly IEmailSender _emailSender;
    private readonly ICacheService _cacheService;

    public CalendarService(ICalendarRepository iCalendarRepository,
                           ILogger<CalendarService> logger,
                           IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IUserProvider userProvider,
                           ICalendarParticipationService calendarParticipationService,
                           IUserService userService,
                           ICalendarParticipationRepository calendarParticipationRepository,
                           IEmailSender emailSender,
                           ICacheService cacheService)
    {
        _iCalendarRepository = iCalendarRepository;
        _iLogger = logger;
        _iMapper = mapper;
        _iUnitOfWork = unitOfWork;
        _userProvider = userProvider;
        _iCalendarParticipationService = calendarParticipationService;
        _iUserService = userService;
        _calendarParticipationRepository = calendarParticipationRepository;
        _emailSender = emailSender;
        _cacheService = cacheService;
    }

    /// <summary>
    /// Tạo Calendar
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<ResponseModel<CalendarResponseModel>> CreateAsync(CalendarModel model)
    {
        try
        {
            var userId = _userProvider.GetUserId();
            var calendar = _iMapper.Map<Calendar>(model);
            calendar.UserId = userId;


            var resultCalendar = await _iCalendarRepository.CreateAsync(calendar);
            await _iUnitOfWork.SaveChangeAsync();

            var calendarParticipation = new CalendarParticipationModel
            {
                CalendarId = resultCalendar.Id,
                UserId = userId,
                Creator = userId
            };

            await _iCalendarParticipationService.CreateAsync(calendarParticipation);

            var dataOut = _iMapper.Map<Calendar, CalendarResponseModel>(resultCalendar);
            dataOut.IsHost = true;
            dataOut.IsSub = false;
            // Save cache
            await _cacheService.SetDataSortedAsync($"{KeyCache.Calendar}:{userId}", new List<CalendarResponseModel>
            {
                dataOut
            });


            return new ResponseModel<CalendarResponseModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, dataOut, new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    /// <summary>
    /// Xoá Calendar
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id)
    {
        try
        {
            var calendar = await _iCalendarRepository.GetByIdAsync(Id);

            _iCalendarRepository.Delete(calendar, true);
            await _iUnitOfWork.SaveChangeAsync();

            var calendarParticipation = await _iCalendarParticipationService.DeleteByCalendarUserIdAsync(new CalendarParticipationGetBycalendarUserIdModel
            {
                UserId = calendar.CreatorId,
                CalendarId = calendar.Id
            });

            return new ResponseModel<CalendarUpdateModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, new CalendarUpdateModel(), new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{Id}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    /// <summary>
    /// Lấy danh sách Calendar đã tạo và đã join
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListCalendarAsync(RequestBaseModel input)
    {
        try
        {
            var userId = _userProvider.GetUserId();

            var dataCache = await _cacheService.GetListDataByKeyRangeIdAsync<CalendarResponseModel>($"{KeyCache.Calendar}:{userId}");

            if(!dataCache.Any())
            {
                var listcalendar = await _iCalendarRepository.GetListCalendarByIdUser(userId,
                                                                                 input.PageNumber,
                                                                                 input.PageSize);

                if (!listcalendar.Any())
                {
                    return new ResponseModel<List<GetListCalendarModel>>(true, "204", new List<GetListCalendarModel>());
                }

                var totalCount = await _iCalendarRepository.CountAsync(x => x.CreatorId.Equals(userId));

                var dataCaches = _iMapper.Map<List<CalendarResponseModel>>(listcalendar);

                await _cacheService.SetDataSortedAsync($"{KeyCache.Calendar}:{userId}", dataCaches);

                return new ResponseModel<List<GetListCalendarModel>>(true,
                                                                     ResponseCode.Donace_BE_Project_CalendarService_Success,
                                                                     _iMapper.Map<List<GetListCalendarModel>>(listcalendar),
                                                                     new(totalCount, input.PageNumber, input.PageSize));

            }
            else
            {
                var calendars = dataCache.Where(x => x.IsSub == false).ToList();
                if (!calendars.Any())
                {
                    return new ResponseModel<List<GetListCalendarModel>>(true, "204", new List<GetListCalendarModel>());
                }

                return new ResponseModel<List<GetListCalendarModel>>(true,
                                                                     ResponseCode.Donace_BE_Project_CalendarService_Success,
                                                                     _iMapper.Map<List<GetListCalendarModel>>(calendars),
                                                                     new(1000000, input.PageNumber, input.PageSize));

            }         
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    public async Task<CalendarResponseModel> GetByIdAsync(Guid id)
    {
        var userId = _userProvider.GetUserId();
        var calendar = await _iCalendarRepository.GetByIdAsync(id);

        if(calendar is null)
        {
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_EventService, "Không tìm thấy Lịch");
        }

        var data = _iMapper.Map<CalendarResponseModel>(calendar);

        if (calendar.UserId == userId)
        {
            data.IsHost = true;
            return data;
        }

        var calendarPart = await _calendarParticipationRepository.FindAsync(x => x.CalendarId == data.Id && 
                                                                           x.UserId == userId &&
                                                                           x.IsDeleted == false);
        if(calendarPart is not null)
        {
            data.IsSub = true;
        }

        return data;
    }

    /// <summary>
    /// Lấy danh sách calendar user subcribed
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListCalendarSubcribedAsync(RequestBaseModel input)
    {
        try
        {
            var userId = _userProvider.GetUserId();
            var ids = await _iCalendarParticipationService.GetListIdCalendarByUserIdAsync();

            if (!ids.Result.Any())
            {
                return new ResponseModel<List<GetListCalendarModel>>(true, "200", new List<GetListCalendarModel>(), new PageInfoModel());
            }

            var dataDb = await _iCalendarRepository.GetListCalendarPagingByIdsAsync(ids.Result, input.PageNumber, input.PageSize);

            var total = await _iCalendarRepository.CountAsync(x => ids.Result.Contains(x.Id));

            var data = _iMapper.Map<List<GetListCalendarModel>>(dataDb);

            foreach(var item in data)
            {
                item.IsSubcribed = true;
            }

            return new ResponseModel<List<GetListCalendarModel>>(true, "200", data, new PageInfoModel(total, input.PageNumber, input.PageSize));
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    /// <summary>
    /// Lấy Danh Sách User Trong Calendar
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<ResponseModel<List<GetListUserInCalendarModel>>> GetListUserInCalendarAsync(RequestGetListUserInCalendarModel input)
    {
        try
        {
            var listUserId = await _iCalendarParticipationService.GetListUserIdOfCalendarAsync(input.Id, input.PageNumber, input.PageSize);
            var total = await _iCalendarParticipationService.TotalUserInCalendarAsync(input.Id);


            if (!listUserId.Result.Any())
            {
                return new ResponseModel<List<GetListUserInCalendarModel>>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, new List<GetListUserInCalendarModel>(), new(0, input.PageNumber, input.PageSize));
            }

            var listUser = await _iUserService.ListUserAsync(listUserId.Result);
            return new ResponseModel<List<GetListUserInCalendarModel>>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, listUser.Result, new(total.Result, input.PageNumber, input.PageSize));
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    /// <summary>
    /// Update calendar
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<ResponseModel<CalendarResponseModel>> UpdateAsync(CalendarUpdateModel model)
    {
        try
        {
            var calendar = await _iCalendarRepository.GetByIdAsync(model.Id);

            var calendarData = _iMapper.Map(model, calendar);

            _iCalendarRepository.Update(calendarData);
            await _iUnitOfWork.SaveChangeAsync();

            var dataout = _iMapper.Map<CalendarUpdateModel, CalendarResponseModel>(model);
            dataout.IsHost = true;
            dataout.IsSub = false;
            // Update cache

            await _cacheService.RemoveItemDataBySortedAsync($"{KeyCache.Calendar}:{calendarData.CreatorId}", calendarData.Sorted);
            await _cacheService.SetDataSortedAsync($"{KeyCache.Calendar}:{calendarData.CreatorId}", new List<CalendarResponseModel>
            {
                dataout
            });

            return new ResponseModel<CalendarResponseModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, dataout, new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    /// <summary>
    /// Join calendar
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task UserJoinCalendarAsync(UserJoinCalendarReqModel input)
    {
        try
        {
            var userId = _userProvider.GetUserId();

            var calendar = await _iCalendarRepository.FindAsync(x => x.Id == input.CalendarId && x.IsDeleted == false);

            if (calendar is null)
            {
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_EventService, "Lịch không tồn tại");
            }

            var checkSub = await _calendarParticipationRepository.FindAsync(x => x.IsDeleted == false &&
                                                                                 x.CalendarId == input.CalendarId &&
                                                                                 x.CreatorId == userId);
            if (checkSub is not null)
            {
                throw new FriendlyException("400", "user đã join vào calendar");
            }

            calendar.TotalSubscriber += 1;

            _iCalendarRepository.Update(calendar);

            await _iCalendarParticipationService.CreateAsync(new CalendarParticipationModel
            {
                UserId = userId,
                CalendarId = input.CalendarId
            }, true);

            await _iUnitOfWork.SaveChangeAsync();
            var listUserjoin = (await _calendarParticipationRepository.ToListAsync(x => x.IsDeleted == false
                                                                                     && x.IsSubcribed == true
                                                                                     && x.CalendarId == input.CalendarId))
                                                                              .Select(x => x.UserId).ToList();

            // Update cache user sub
            var dataCache = _iMapper.Map<CalendarResponseModel>(calendar);
            dataCache.IsSub = true;
            dataCache.IsHost = false;
            foreach (var id in listUserjoin)
            {
                await _cacheService.RemoveItemDataBySortedAsync($"{KeyCache.Calendar}:{id}", dataCache.Sorted);
                await _cacheService.UpdateValueScoreAsync($"{KeyCache.Calendar}:{id}", calendar.Sorted, dataCache);
            }
            // update cache user host
            dataCache.IsHost = true;
            dataCache.IsSub = false;

            await _cacheService.RemoveItemDataBySortedAsync($"{KeyCache.Calendar}:{calendar.CreatorId}", dataCache.Sorted);
            await _cacheService.UpdateValueScoreAsync($"{KeyCache.Calendar}:{calendar.CreatorId}", dataCache.Sorted, dataCache);
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"Calendar.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    public async Task InviteJoinCalendarAsync(InviteJoinCalendarModel input)
    {
        try
        {
            var sub = $"Mời bạn tham gia lịch {input.CalendarName}";

            var body = $"Click vào link {input.UrlInfo} để xem thông tin";

            await _emailSender.SendEmailAsync(input.Email, sub, body);
        }
        catch(FriendlyException ex)
        {
            _iLogger.LogError($"InviteJoinCalendar exception: {ex.Message}", JsonConvert.SerializeObject(input));
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }
}
