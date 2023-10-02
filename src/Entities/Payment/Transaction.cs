using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.Enums.Entity;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Payment;

public class Transaction : BaseEntity
{
    [Required]
    public OrderStatus Status { get; set; }

    [Required]
    public Guid OrderId { get; set; }

    public string ExtraData { get; set; } = string.Empty;
}
