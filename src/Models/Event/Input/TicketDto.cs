namespace Donace_BE_Project.Models.Event.Input
{
    public class TicketDto
    {
        public bool IsRequireApprove { get; set; }
        public bool IsFree { get; set; }
        public decimal Price { get; set; }
    }
}
