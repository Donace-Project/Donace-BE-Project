using Donace_BE_Project.Entities.Payment;

namespace Donace_BE_Project.Interfaces.Repositories
{
    public interface IConnectPaymentRepository : IRepositoryBase<ConnectPayment>
    {
        Task<ConnectPayment> GetByUserAsync(Guid userId);
    }
}
