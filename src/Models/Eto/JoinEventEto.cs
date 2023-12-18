namespace Donace_BE_Project.Models.Eto
{
    public class JoinEventEto : BaseEto
    {
        public Guid UserId { get; set; }
        public Guid EventId { get; set; }
    }
}
