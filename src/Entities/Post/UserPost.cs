using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.Post;

public class UserPost : BaseEntity
{
    [Required]
    public bool Liked { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid PostId { get; set; }
}
