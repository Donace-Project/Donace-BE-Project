using Donace_BE_Project.Interfaces.Services.Event;
using Donace_BE_Project.Models.Event.Output;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventPublicController : ControllerBase
    {
        private readonly IEventService _service;
        public EventPublicController(IEventService eventService)
        {
            _service = eventService;
        }

        [HttpGet("detail-by-id")]
        public async Task<EventDetailModel> GetDetailById(Guid id)
        {
            return await _service.GetDetailByIdAsync(id);
        }
    }
}
