using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Calendar;

public class Section : BaseEntity
{
    [Required]
    public DateTime StarDate { get; set; }

    [Required]
    public Guid EventId { get; set; }
}
