﻿using AutoMapper;
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
    public CalendarParticipationService(ICalendarParticipationRepository calendarParticipationRepository,
                                        ILogger<CalendarParticipationService> logger,
                                        IMapper mapper,
                                        IUnitOfWork unitOfWork)
    {
        _iCalendarParticipationRepository = calendarParticipationRepository;
        _iLogger = logger;
        _iMapper = mapper;
        _iUnitOfWork = unitOfWork;
    }
    public async Task<ResponseModel<CalendarParticipationModel>> CreateAsync(CalendarParticipationModel model)
    {
        try
        {
            var calendarParti = _iMapper.Map<CalendarParticipation>(model);

            await _iCalendarParticipationRepository.CreateAsync(calendarParti);
            await _iUnitOfWork.SaveChangeAsync();
            return new ResponseModel<CalendarParticipationModel>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, model);
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
                return new ResponseModel<CalendarParticipationGetByCalendarIdModel>(true, ResponseCode.Donace_BE_Project_CalendarParticipationService_Success, new());
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
}
