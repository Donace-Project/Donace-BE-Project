namespace Donace_BE_Project.Models.Ticket
{
    public class UserTicketCheckInModel
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }
    }
    public class UserTicketScanModel
    {
        public string Message { get; set; }
    }
}
