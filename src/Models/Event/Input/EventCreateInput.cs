using Donace_BE_Project.Entities.Ticket;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.Event.Input;

public class EventCreateInput
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string AddressName { get; set; } = string.Empty;

    public float? Lat { get; set; }

    public float? Long { get; set; }

    public bool IsUnlimited { get; set; }
    public int Capacity { get; set; }

    public bool IsOverCapacity { get; set; }

    public string Cover { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Theme { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public float FontSize { get; set; }

    public string Instructions { get; set; } = string.Empty;

    public bool IsMultiSection { get; set; }

    public float Duration { get; set; }
    public bool IsOnline { get; set; }
    public string LinkMeet { get; set; } = string.Empty;
    public List<SectionCreateInput> Sections { get; set; } = new();
    public Guid CalendarId { get; set; }
    public string Desc { get; set; } = string.Empty;

    // TODO: lưu ticket
    public TicketDto Ticket { get; set; }
}

public class SectionCreateInput
{
    public Guid Id { get; set; }

    [Required]
    public DateTime StarDate { get; set; }
}

public class GetListEventCalendarModel : RequestBaseModel
{
    public Guid CalendarId { get; set; }
    public bool IsUpcoming { get; set; }    
}

public class UserJoinEventModel
{
    public Guid UserId { get; set; }
    public Guid? CalendarId { get; set; }
    public Guid EventId { get; set; }
}