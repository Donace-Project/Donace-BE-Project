using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Post;

public class Post : BaseEntity
{
    public string Message { get; set; } = string.Empty;
    public int TotalLike { get; set; }

    public Guid CreatedUserId { get; set; }
    public Guid EventId { get; set; }
}
