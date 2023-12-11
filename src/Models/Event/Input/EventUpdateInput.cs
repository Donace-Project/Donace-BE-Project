using Donace_BE_Project.Enums.Entity;

namespace Donace_BE_Project.Models.Event.Input;

public class EventUpdateInput : EventCreateInput
{
    public Guid Id { get; set; }
}

public class ApprovalEventInput
{
    public Guid IdPart { get; set; }
    public EventParticipationStatus Status { get; set; }
    public string Qr { get; set; }
    public Guid UserId { get; set; }
}