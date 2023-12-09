using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Ticket;

namespace Donace_BE_Project.Services
{
    public class UserTicketsService : IUserTicketsService
    {
        private readonly IUserTicketsRepository _userTicketsRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketsRepository _ticketsRepository;

        public UserTicketsService(IUserTicketsRepository userTicketsRepository,
                                  IEventRepository eventRepository,
                                  ITicketsRepository ticketsRepository)
        {
            _userTicketsRepository = userTicketsRepository;
            _eventRepository = eventRepository;
            _ticketsRepository = ticketsRepository;
        }
        public async Task<UserTicketScanModel> CheckInAsync(UserTicketCheckInModel input)
        {
            try
            {
                var checkEvent = await _ticketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                                       x.EventId == input.EventId);  
                
                if(checkEvent is null) 
                {
                    throw new FriendlyException("404", "Không tìm thấy event");
                }

                var ticket = await _userTicketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                                         x.TicketId == checkEvent.Id);

                if(ticket is null)
                {
                    throw new FriendlyException("404", "Ticket không tồn tại");
                }
                if (!ticket.IsChecked)
                {
                    throw new FriendlyException("400", "Ticket đã được dùng");
                }

                ticket.IsChecked = true;
                _userTicketsRepository.Update(ticket);

                return new UserTicketScanModel
                {
                    Message = "Checkin thành công"
                };
            }
            catch(FriendlyException ex)
            {
                throw new FriendlyException("500", ex.Message);
            }
        }
    }
}
