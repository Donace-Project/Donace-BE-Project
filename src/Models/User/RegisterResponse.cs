using Microsoft.AspNetCore.Identity;

namespace Donace_BE_Project.Models.User
{
    public class RegisterResponse
    {
        public IdentityResult Result { get; set; } = default!;
        public string Token { get; set; } = string.Empty;
        public UserModel User { get; set; } = default!;
    }
}
