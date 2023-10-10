using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.CalendarParticipation
{
    public class CalendarParticipationModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid CalendarId { get; set; }
        public Guid Creator {  get; set; }
    }

    public class CalendarParticipationGetByCalendarIdModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid CalendarId { get; set; }
        public Guid Creator { get; set; }
    }
}
