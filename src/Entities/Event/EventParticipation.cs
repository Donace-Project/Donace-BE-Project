using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.Enums.Entity;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Event;

public class EventParticipation : BaseEntity
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid EventId { get; set; }

    public EventParticipationStatus Status { get; set; }
}
