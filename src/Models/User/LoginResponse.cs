namespace Donace_BE_Project.Models.User
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserModel User { get; set; } = default!;
    }
}
