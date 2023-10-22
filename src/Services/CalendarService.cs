using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.CalendarParticipation;
using Newtonsoft.Json;

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
    private readonly ICacheService _iCacheService;
    public CalendarService(ICalendarRepository iCalendarRepository,
                           ILogger<CalendarService> logger,
                           IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IUserProvider userProvider,
                           ICalendarParticipationService calendarParticipationService,
                           IUserService userService,
                           ICacheService iCacheService)
    {
        _iCalendarRepository = iCalendarRepository;
        _iLogger = logger;
        _iMapper = mapper;
        _iUnitOfWork = unitOfWork;
        _userProvider = userProvider;
        _iCalendarParticipationService = calendarParticipationService;
        _iUserService = userService;
        _iCacheService = iCacheService;
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

            await _iCacheService.SetListDataSortedAsync($"{KeyCache.Calendar}:{userId}", dataOut);

            return new ResponseModel<CalendarResponseModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, dataOut, new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    public async Task<ResponseModel<CalendarUpdateModel>> DeleteAsync(Guid Id)
    {
        try
        {
            var calendar = await _iCalendarRepository.GetByIdAsync(Id);

            _iCalendarRepository.Delete(calendar,true);
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

    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListCalendarAsync(RequestBaseModel input)
    {
        try
        {
            var userId = _userProvider.GetUserId();
            var litsCalendarId = await _iCalendarParticipationService.GetListIdCalendarByUserIdAsync();

            if (!litsCalendarId.Result.Any())
            {
                return new ResponseModel<List<GetListCalendarModel>>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, new List<GetListCalendarModel>(), new());
            }

            var listcalendar = await _iCalendarRepository.GetListCalendarByIds(litsCalendarId.Result, input.PageNumber, input.PageSize);
            var totalCount = await _iCalendarRepository.CountAsync(x => litsCalendarId.Result.Contains(x.Id));
            var result = _iMapper.Map<List<GetListCalendarModel>>(listcalendar);

            foreach(var item in result)
            {
                item.IsAdmin = item.UserId == userId ? true : false;
            }
            return new ResponseModel<List<GetListCalendarModel>>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, result, new(totalCount, input.PageNumber, input.PageSize));
            
        }
        catch (Exception ex) 
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    public async Task<ResponseModel<List<GetListUserInCalendarModel>>> GetListUserInCalendarAsync(RequestGetListUserInCalendarModel input)
    {
        try
        {
            var listUserId = await _iCalendarParticipationService.GetListUserIdOfCalendarAsync(input.Id, input.PageNumber, input.PageSize);
            var total = await _iCalendarParticipationService.TotalUserInCalendarAsync(input.Id);


            if (!listUserId.Result.Any())
            {
                return new ResponseModel<List<GetListUserInCalendarModel>>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, new List<GetListUserInCalendarModel>(), new(0,input.PageNumber, input.PageSize));
            }

            var listUser = await _iUserService.ListUserAsync(listUserId.Result);
            return new ResponseModel<List<GetListUserInCalendarModel>>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, listUser.Result, new(total.Result, input.PageNumber, input.PageSize));
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    public async Task<ResponseModel<CalendarUpdateModel>> UpdateAsync(CalendarUpdateModel model)
    {
        try
        {
            var calendar = await _iCalendarRepository.GetByIdAsync(model.Id);

            var calendarData = _iMapper.Map(model, calendar);

            _iCalendarRepository.Update(calendarData);
            await _iUnitOfWork.SaveChangeAsync();

            return new ResponseModel<CalendarUpdateModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, model, new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }
}
