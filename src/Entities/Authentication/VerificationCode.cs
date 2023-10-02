using System.ComponentModel.DataAnnotations;
using Donace_BE_Project.Entities.Base;
using Donace_BE_Project.Enums.Entity;

namespace Donace_BE_Project.Entities.Authentication;

public class VerificationCode : BaseEntity
{
    [Required]
    [MaxLength(6)]
    public required string Code { get; set; }

    [Required]
    public VerificationCodeStatus Status { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
