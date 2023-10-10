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
    public CalendarService(ICalendarRepository iCalendarRepository,
                           ILogger<CalendarService> logger,
                           IMapper mapper,
                           IUnitOfWork unitOfWork,
                           IUserProvider userProvider,
                           ICalendarParticipationService calendarParticipationService)
    {
        _iCalendarRepository = iCalendarRepository;
        _iLogger = logger;
        _iMapper = mapper;
        _iUnitOfWork = unitOfWork;
        _userProvider = userProvider;
        _iCalendarParticipationService = calendarParticipationService;
    }

    public async Task<ResponseModel<CalendarModel>> CreateAsync(CalendarModel model)
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

            return new ResponseModel<CalendarModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, model);
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

            return new ResponseModel<CalendarUpdateModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, new());
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{Id}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }

    public async Task<ResponseModel<CalendarUpdateModel>> UpdateAsync(CalendarUpdateModel model)
    {
        try
        {
            var calendarData = _iMapper.Map<Calendar>(model);

            _iCalendarRepository.Update(calendarData);
            await _iUnitOfWork.SaveChangeAsync();

            return new ResponseModel<CalendarUpdateModel>(true, ResponseCode.Donace_BE_Project_CalendarService_Success, model);
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"CalendarService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarService, ex.Message);
        }
    }
}
