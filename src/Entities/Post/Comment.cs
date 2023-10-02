using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Post;

public class Comment : BaseEntity
{
    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid PostId { get; set; }
}
