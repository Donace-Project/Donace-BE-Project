using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces;
using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HandleExceptionTestController : ControllerBase
{

    public HandleExceptionTestController()
    {
    }

    [HttpGet("profile")]
    public Task<UserModel> Profile(Guid userId)
    {
        throw new FriendlyException("404", "không không tìm thấy user")
            .WithData("UserId", userId);
    }
}
