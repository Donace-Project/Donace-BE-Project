using Donace_BE_Project.Interfaces.Services;
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

    [HttpPost("register")]
    public async Task<IActionResult> Register(UserDto input)
    {
        var result = await _service.RegisterAsync(input);

        return !result.Succeeded ? new BadRequestObjectResult(result.Errors)
            : StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<LoginResponse> LoginAsync(UserDto input)
    {
        return await _service.LoginAsync(input);
    }
}
