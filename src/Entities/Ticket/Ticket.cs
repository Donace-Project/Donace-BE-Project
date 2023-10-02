using Donace_BE_Project.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Ticket;

public class Ticket : BaseEntity
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public bool IsRequireApprove { get; set; }

    [Required]
    public bool IsFree { get; set; }

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public decimal Total { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public int Index { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public bool IsRestrictDate { get; set; }

    [Required]
    public Guid EventId { get; set; }
}
