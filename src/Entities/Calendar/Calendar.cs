using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Calendar;

public class Calendar : BaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string Cover { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string PublicURL { get; set; } = string.Empty;

    public string Lat { get; set; } = string.Empty;

    public string Long { get; set; } = string.Empty;

    public string AddressName { get; set; } = string.Empty;

    public int TotalSubscriber { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Sorted { get; set; }
}
