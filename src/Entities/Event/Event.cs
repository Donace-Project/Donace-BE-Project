using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Calendar;

public class Event : BaseEntity
{
    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string AddressName { get; set; } = string.Empty;

    public float? Lat { get; set; }

    public float? Long { get; set; }

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

    public int TotalGuest { get; set; }

    public List<Section> Sections { get; set; } = default!;

    public Guid CalendarId { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Sorted { get; set; }

    public string LocationCode { get; set; } = string.Empty;

    public bool IsOnline { get; set; }
    public string LinkMeet { get; set; } = string.Empty;
}
