using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Notification;

public class Notification : BaseEntity
{
    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }
}
