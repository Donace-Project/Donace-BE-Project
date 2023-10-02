using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;

namespace Donace_BE_Project.Entities.User;

public class User : BaseEntity
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string Bio { get; set; } = string.Empty;

    public string Instagram { get; set; } = string.Empty;

    public string Twitter { get; set; } = string.Empty;

    public string Youtube { get; set; } = string.Empty;

    public string Tiktok { get; set; } = string.Empty;

    public string LinkedIn { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public string Uid { get; set; } = string.Empty;
}
