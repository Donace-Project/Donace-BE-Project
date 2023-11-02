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
using System.Net.WebSockets;

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
    public CalendarService(ICalendarRepository iCalendarRepository,
                           ILogger<CalendarService> logger,
                           IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IUserProvider userProvider,
                           ICalendarParticipationService calendarParticipationService,
                           IUserService userService)
    {
        _iCalendarRepository = iCalendarRepository;
        _iLogger = logger;
        _iMapper = mapper;
        _iUnitOfWork = unitOfWork;
        _userProvider = userProvider;
        _iCalendarParticipationService = calendarParticipationService;
        _iUserService = userService;
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

    /// <summary>
    /// Lấy danh sách Calendar
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="FriendlyException"></exception>
    public async Task<ResponseModel<List<GetListCalendarModel>>> GetListCalendarAsync(RequestBaseModel input)
    {
        try
        {
            var result = new List<GetListCalendarModel>();
            var userId = _userProvider.GetUserId();

            var listcalendar = await _iCalendarRepository.GetListCalendarByIdUser(userId, 
                                                                                  input.PageNumber, 
                                                                                  input.PageSize);

            if (!listcalendar.Any())
            {
                return new ResponseModel<List<GetListCalendarModel>>(true, "204", new List<GetListCalendarModel>());
            }

            var totalCount = await _iCalendarRepository.CountAsync(x => x.CreatorId.Equals(userId));
            result = _iMapper.Map<List<GetListCalendarModel>>(listcalendar);

            return new ResponseModel<List<GetListCalendarModel>>(true, 
                                                                 ResponseCode.Donace_BE_Project_CalendarService_Success, 
                                                                 result, 
                                                                 new(totalCount, input.PageNumber, input.PageSize));
            
        }
        catch (Exception ex) 
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
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

            return new ResponseModel<List<GetListCalendarModel>>(true, "200", data, new PageInfoModel(total, input.PageNumber, input.PageSize));
        }
        catch(Exception ex)
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
            await _iCalendarParticipationService.CreateAsync(new CalendarParticipationModel
            {
                UserId = userId,
                CalendarId = input.CalendarId
            }, true);            
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"Calendar.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(input)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }
}
