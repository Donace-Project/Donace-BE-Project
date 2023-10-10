using System.ComponentModel.DataAnnotations;

namespace Donace_BE_Project.Models.User
{
    public class UserDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
}
