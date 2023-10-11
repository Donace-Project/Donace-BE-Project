namespace Donace_BE_Project.Models.User;

public class UserModel
{
    public Guid? Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Bio {  get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Youtube { get; set; } = string.Empty;
    public string Tiktok { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}

public class UpdateUserModel
{
    public string UserName { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Youtube { get; set; } = string.Empty;
    public string Tiktok { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
}
