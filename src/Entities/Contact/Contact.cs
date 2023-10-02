using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Contact;

public class Contact : BaseEntity
{
    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public Guid ReceivedUserId { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
