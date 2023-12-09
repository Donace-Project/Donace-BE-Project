using Donace_BE_Project.Models;
using Donace_BE_Project.Models.VNPay;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<ResponseModel<bool>> ConnectPaymentVnPayAsync(ConnectVnPayModel input);
        Task<ConnectVnPayModel> GetConnectAsync();
    }
}
