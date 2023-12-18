using Donace_BE_Project.Enums.Entity;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.Event.Input;

public class EventUpdateInput : BaseEventInput
{
    public Guid Id { get; set; }
}

public class EventForUpdateCover
{
    [Required]
    public Guid Id { get; set;}

    [Required]
    public string Cover {  get; set; }
}

public class ApprovalEventInput
{
    public Guid IdPart { get; set; }
    public EventParticipationStatus Status { get; set; }
    public string Qr { get; set; }
    public Guid UserId { get; set; }
    public bool IsApproved { get; set; } = true;
}