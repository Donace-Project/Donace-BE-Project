using Donace_BE_Project.Models.Oder;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ResponsePayment> CreateOrderAsync(OrderModel input);

        Task<ResponsePayment> ContinuePaymentAsync(Guid input);

        Task<OrderModel> CallBackAsync(Guid id);
    }
}
