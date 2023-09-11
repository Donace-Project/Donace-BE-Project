namespace Donace_BE_Project.Entities;

public class BaseEntity<T>
{
    public T Id { get; set; }
    public DateTime CreationTime { get; set; } = DateTime.Now;
    public Guid CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public Guid? LastModifierId { get; set; }
    public bool IsDeleted { get; set; }
}
