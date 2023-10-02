using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Calendar;

public class Event : BaseEntity
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

    public Guid CalendarId { get; set; }
}
