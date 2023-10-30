using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationAppService _service;

    public AuthenticationController(IAuthenticationAppService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<RegisterResponse> Register(UserDto input)
    {
        var result = await _service.RegisterAsync(input);
        return result;
    }

    [HttpPost("login")]
    public async Task<LoginResponse> LoginAsync(UserDto input)
    {
        return await _service.LoginAsync(input);
    }
}
