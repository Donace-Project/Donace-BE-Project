using Donace_BE_Project.Models.Ticket;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IUserTicketsService
    {
        Task<UserTicketScanModel> CheckInAsync(UserTicketCheckInModel input);
    }
}
