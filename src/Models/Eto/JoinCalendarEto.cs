namespace Donace_BE_Project.Models.Eto
{
    public class JoinCalendarEto : BaseEto
    {
        public Guid CalendarId { get; set; }
    }
    public class BaseEto
    {
        public string Token { get; set; }
    }
}
