namespace Donace_BE_Project.Models.Calendar
{
    public class UserJoinCalendarReqModel
    {
        public Guid CreatorId { get; set; }
        public int Sorted {  get; set; }
        public Guid CalendarId { get; set; }
    }
}
