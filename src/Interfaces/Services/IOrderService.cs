using Donace_BE_Project.Models.Oder;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IOrderService
    {
        Task CreateOrderAsync(OrderModel input);
    }
}
