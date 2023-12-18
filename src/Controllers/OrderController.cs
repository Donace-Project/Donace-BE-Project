using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.Oder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("create")]
        public async Task<ResponsePayment> CreateAsync(OrderModel input)
        {
            return await _orderService.CreateOrderAsync(input);
        }

        [HttpPost("continue")]
        public async Task<ResponsePayment> OrderContinueAsync(Guid id)
        {
            return await _orderService.ContinuePaymentAsync(id);
        }

        [HttpPost("call-back")]
        public async Task<OrderModel> CallBackAsync(Guid id)
        {
            return await _orderService.CallBackAsync(id);
        }
    }
}
