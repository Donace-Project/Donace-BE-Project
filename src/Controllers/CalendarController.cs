﻿using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CalendarController : ControllerBase
{
    private readonly ICalendarService _iCalendarService;
    public CalendarController(ICalendarService calendar)
    {
        _iCalendarService = calendar;
    }

    [HttpPost("create-calendar")]
    public async Task<ResponseModel<CalendarModel>> CreateAsync(CalendarModel model)
    {
        return await _iCalendarService.CreateAsync(model);
    }
}
