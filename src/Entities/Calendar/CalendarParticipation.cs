using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Calendar;

public class CalendarParticipation : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid CalendarId { get; set; }

    [Required]
    public bool IsSubcribed {  get; set; }
}
