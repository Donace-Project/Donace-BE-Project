using AutoMapper;
using Donace_BE_Project.Entities.Payment;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Oder;
using Donace_BE_Project.Shared;
using Nest;
using System.Net.WebSockets;

namespace Donace_BE_Project.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketsRepository _ticketsRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConnectPaymentRepository _connectPaymentRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public OrderService(IOrderRepository orderRepository,
                            IMapper mapper,
                            IEventRepository eventRepository,
                            ITicketsRepository ticketsRepository,
                            IUnitOfWork unitOfWork,
                            IConnectPaymentRepository connectPaymentRepository,
                            IHttpContextAccessor contextAccessor)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _eventRepository = eventRepository;
            _ticketsRepository = ticketsRepository;
            _unitOfWork = unitOfWork;
            _connectPaymentRepository = connectPaymentRepository;
            _contextAccessor = contextAccessor;

        }

        public async Task<string> CreateOrderAsync(OrderModel input)
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

                var result = await _orderRepository.CreateAsync(data);

                await _unitOfWork.SaveChangeAsync();

                return await ContinuePaymentAsync(result.Id);

            }
            catch (FriendlyException ex)
            {
                await _unitOfWork.RollbackAsync();
                throw new FriendlyException("500", ex.Message);
            }
        }

        public async Task<string> ContinuePaymentAsync(Guid id)
        {
            try
            {
                var order = await _orderRepository.FindAsync(x => x.Id == id && 
                                                                  x.IsDeleted == false &&
                                                                  x.Status != Enums.Entity.OrderStatus.Paid);

                if(order is null)
                {
                    throw new FriendlyException("404", $"Không tìm thấy order: {id}");
                }



                var userHost = await _ticketsRepository.FindAsync(x => x.Id == order.TicketId &&
                                                                       x.IsDeleted == false &&
                                                                       x.IsFree == false);

                if(userHost is null)
                {
                    throw new FriendlyException("404", "Không tìm thấy thông tin ticket cần thanh toán");
                }

                var paymentManager = await _connectPaymentRepository.FindAsync(x => x.CreatorId == userHost.CreatorId &&
                                                                                    x.IsDeleted == false);

                if(paymentManager is null)
                {
                    throw new FriendlyException("404", "Không tìm thấy thông tin thanh toán");
                }

                string url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string returnUrl = "https://localhost:3000/payment/success";

                VnPayLibrary pay = new VnPayLibrary();

                pay.AddRequestData("vnp_Version", "2.1.0");
                pay.AddRequestData("vnp_Command", "pay");
                pay.AddRequestData("vnp_TmnCode", paymentManager.Key);
                pay.AddRequestData("vnp_Amount", "1000000");
                pay.AddRequestData("vnp_BankCode", "");
                pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                pay.AddRequestData("vnp_CurrCode", "VND");
                pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress(_contextAccessor.HttpContext));
                pay.AddRequestData("vnp_Locale", "vn");
                pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
                pay.AddRequestData("vnp_OrderType", "other");
                pay.AddRequestData("vnp_ReturnUrl", returnUrl);
                pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

                return pay.CreateRequestUrl(url, paymentManager.SecretKey);
            }
            catch(FriendlyException ex)
            {
                throw new FriendlyException("400", ex.Message);
            }
        }

        public async Task<OrderModel> CallBackAsync(Guid id)
        {
            try
            {
                var order = await _orderRepository.FindAsync(x => x.Id == id && x.IsDeleted == false);

                order.Status = Enums.Entity.OrderStatus.Paid;

                _orderRepository.Update(order);

                await _unitOfWork.SaveChangeCusAsync();

                return _mapper.Map<OrderModel>(order);
            }
            catch(FriendlyException ex)
            {
                throw new FriendlyException("400", ex.Message);
            }
        }
    }
}
