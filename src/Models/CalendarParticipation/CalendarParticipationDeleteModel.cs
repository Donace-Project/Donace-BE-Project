using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.CalendarParticipation
{
    public class CalendarParticipationGetBycalendarUserIdModel
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid CalendarId {  get; set; }
    }
}
