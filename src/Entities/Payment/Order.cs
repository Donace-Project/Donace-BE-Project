using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.Enums.Entity;

namespace Donace_BE_Project.Entities.Payment;

public class Order : BaseEntity
{
    [Required]
    public decimal TotalPrice { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    [Required]
    public Guid PaymentMethodId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TicketId { get; set; }
}
