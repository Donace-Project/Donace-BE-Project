using AutoMapper;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Oder;
using System.Net.WebSockets;

namespace Donace_BE_Project.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketsRepository _ticketsRepository;

        public OrderService(IOrderRepository orderRepository,
                            IMapper mapper,
                            IEventRepository eventRepository,
                            ITicketsRepository ticketsRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _ticketsRepository = ticketsRepository;
        }

        public async Task CreateOrderAsync(OrderModel input)
        {
            try
            {
                var ticket = await _ticketsRepository.FindAsync(x => x.Id == input.TicketId &&
                                                                     x.IsDeleted == false);

                if (ticket is null) 
                {
                    throw new FriendlyException("400", "Ticket không tồn tại");
                }

                input.Status = Enums.Entity.OrderStatus.Processing;

                var data = _mapper.Map<Order>(input);

                await _orderRepository.CreateAsync(data);


            }
            catch (FriendlyException ex)
            {
                throw new FriendlyException("500", ex.Message);
            }
        }

        private async Task<>
    }
}
