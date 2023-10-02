using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Contact;

public class InvitedUser : BaseEntity
{
    [Required]
    public Guid InvitedUserId { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
