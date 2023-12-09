using Donace_BE_Project.Enums.Entity;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.Oder
{
    public class OrderModel
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
}
