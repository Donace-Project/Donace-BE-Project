using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Ticket;

public class UserTicket : BaseEntity
{
    [Required]
    public string QrCode { get; set; } = string.Empty;

    [Required]
    public bool IsChecked { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TicketId { get; set; }
}
