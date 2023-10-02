using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Entities.Base;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    public DateTime CreationTime { get; set; }

    public Guid CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public bool IsDeleted { get; set; } = false;

    public bool IsEnable { get; set; } = true;
}
