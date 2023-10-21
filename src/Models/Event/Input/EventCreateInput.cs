using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.Event.Input;

public class EventCreateInput
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string AddressName { get; set; } = string.Empty;

    public string Lat { get; set; } = string.Empty;

    public string Long { get; set; } = string.Empty;

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

    public List<SectionCreateInput> Sections { get; set; } = new();

    public Guid CalendarId { get; set; }
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