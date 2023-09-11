using Donace_BE_Project.Interfaces;
using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("profile")]
    public Task<UserModel> Profile(Guid id)
    {
        return _service.GetProfileAsync(id);
    }
}
