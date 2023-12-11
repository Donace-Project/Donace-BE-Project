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
        private readonly IUserProvider _userProvider;
        private readonly IUnitOfWork _unitOfWork;
        public UserTicketsService(IUserTicketsRepository userTicketsRepository,
                                  IEventRepository eventRepository,
                                  ITicketsRepository ticketsRepository,
                                  IUserProvider userProvider,
                                  IUnitOfWork unitOfWork)
        {
            _userTicketsRepository = userTicketsRepository;
            _eventRepository = eventRepository;
            _ticketsRepository = ticketsRepository;
            _userProvider = userProvider;
            _unitOfWork = unitOfWork;
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
                if (ticket.IsChecked)
                {
                    throw new FriendlyException("400", "Ticket đã được dùng");
                }

                ticket.IsChecked = true;
                _userTicketsRepository.Update(ticket);
                await _unitOfWork.SaveChangeAsync();

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

        public async Task<Guid> GetTicketAsync()
        {
            try
            {
                var userId = _userProvider.GetUserId();
                return (await _userTicketsRepository.FindAsync(x => x.IsDeleted == false &&
                                                                   x.UserId == userId)).Id;
            }
            catch(FriendlyException ex)
            {
                throw new FriendlyException("400", ex.Message);
            }
        }
    }
}
