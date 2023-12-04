using Donace_BE_Project.Enums.Entity;

namespace Donace_BE_Project.Models.EventParticipation
{
    public class EventParticipationModel
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; }
        public EventParticipationStatus Status { get; set; }
    }

    public class EventParticipationApprovalModel : EventParticipationModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
