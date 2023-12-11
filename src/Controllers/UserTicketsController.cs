using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Ticket;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTicketsController : ControllerBase
    {
        private readonly IUserTicketsService _userTicketsService;
        public UserTicketsController(IUserTicketsService userTicketsService)
        {
            _userTicketsService = userTicketsService;
        }

        [HttpPost("Check-in")]
        public async Task<UserTicketScanModel> CheckInAsync(UserTicketCheckInModel input)
        {
            return await _userTicketsService.CheckInAsync(input);
        }

        [HttpGet("get-ticket")]
        public async Task<Guid> GetTicketAsync()
        {
            return await _userTicketsService.GetTicketAsync();
        }
    }
}
