using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.Calendar;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.CalendarParticipation;
using Newtonsoft.Json;

namespace Donace_BE_Project.Services;

public class CalendarParticipationService : ICalendarParticipationService
{
    private readonly ICalendarParticipationRepository _iCalendarParticipationRepository;
    private readonly ILogger<CalendarParticipationService> _iLogger;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IUserProvider _iUserProvider;
    public CalendarParticipationService(ICalendarParticipationRepository calendarParticipationRepository,
                                        ILogger<CalendarParticipationService> logger,
                                        IMapper mapper,
                                        IUnitOfWork unitOfWork,
                                        IUserProvider iUserProvider)
    {
        _iCalendarParticipationRepository = calendarParticipationRepository;
        _iLogger = logger;
        _iMapper = mapper;
        _iUnitOfWork = unitOfWork;
        _iUserProvider = iUserProvider;
    }
    public async Task<ResponseModel<CalendarParticipationModel>> CreateAsync(CalendarParticipationModel model, bool isSubcribed = false)
    {
        try
        {
            var calendarParti = _iMapper.Map<CalendarParticipation>(model);
            calendarParti.IsSubcribed = isSubcribed;
            await _iCalendarParticipationRepository.CreateAsync(calendarParti);
            await _iUnitOfWork.SaveChangeAsync();

            return new ResponseModel<CalendarParticipationModel>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, model, new());
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"CalendarParticipationService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarParticipationService, ex.Message);
        }
    }

    public async Task<ResponseModel<CalendarParticipationGetByCalendarIdModel>> DeleteByCalendarUserIdAsync(CalendarParticipationGetBycalendarUserIdModel model)
    {
        try
        {
            var calendarParticipation = await _iCalendarParticipationRepository.FindAsync(x => x.UserId == model.UserId &&
                                                                                               x.CalendarId == model.CalendarId);

            if(calendarParticipation != null)
            {
                _iCalendarParticipationRepository.Delete(calendarParticipation, true);
                await _iUnitOfWork.SaveChangeAsync();
                return new ResponseModel<CalendarParticipationGetByCalendarIdModel>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, new CalendarParticipationGetByCalendarIdModel(), new());
            }

            _iLogger.LogWarning($"CalendarParticipationService.Warning: Not Found CalendarParticipation", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_CalendarParticipationService, "Not Found CalendarParticipation");
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"CalendarParticipationService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarParticipationService , ex.Message);
        }
    }

    public async Task<ResponseModel<List<Guid>>> GetListIdCalendarByUserIdAsync()
    {
        try
        {
            var userId = _iUserProvider.GetUserId();
            var listId = await _iCalendarParticipationRepository.GetListCalendarIdAsync(x => x.UserId == userId &&
                                                                                             x.IsDeleted == false &&
                                                                                             x.IsSubcribed == false);

            return new ResponseModel<List<Guid>>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, listId, new());
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"CalendarParticipationService.Exception: {ex.Message}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarParticipationService, ex.Message);
        }
    }

    public async Task<ResponseModel<List<Guid>>> GetListUserIdOfCalendarAsync(Guid idCalendar, int pageNumber, int pageSize)
    {
        try
        {
            var listUserId = await _iCalendarParticipationRepository.GetListUserIdOfCalendarAsync(x => x.CalendarId == idCalendar, pageNumber, pageSize);

            return new ResponseModel<List<Guid>>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, listUserId, new());
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"CalendarParticipationService.Exception: {ex.Message}", $"Payload: {idCalendar}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarParticipationService, ex.Message);
        }
    }

    public async Task<ResponseModel<long>> TotalUserInCalendarAsync(Guid idCalendar)
    {
        try
        {
            var total = await _iCalendarParticipationRepository.CountAsync(x => x.CalendarId == idCalendar && x.IsDeleted == false);
            return new ResponseModel<long>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, total, new());
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"CalendarParticipationService.Exception: {ex.Message}", $"Payload: {idCalendar}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CalendarParticipationService, ex.Message);
        }
    }
}
