using Microsoft.AspNetCore.Identity;

namespace Donace_BE_Project.Entities.User;

public class User : IdentityUser
{
    public string Avatar { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string Instagram { get; set; } = string.Empty;

    public string Twitter { get; set; } = string.Empty;

    public string Youtube { get; set; } = string.Empty;

    public string Tiktok { get; set; } = string.Empty;

    public string LinkedIn { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public DateTime CreationTime { get; set; }

    public Guid CreatorId { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public Guid? LastModifierId { get; set; }

    public bool IsDeleted { get; set; } = false;

    public bool IsEnable { get; set; } = true;
}
