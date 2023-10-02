using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Payment;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
