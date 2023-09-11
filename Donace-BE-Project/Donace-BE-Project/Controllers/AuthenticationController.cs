using Donace_BE_Project.Interfaces;
using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IUserService _service;

    public AuthenticationController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public Task<UserModel> Register(string email)
    {
        return _service.RegisterAsync(email);
    }

    [HttpGet("profile")]
    public Task<UserModel> Profile(Guid id)
    {
        return _service.GetProfileAsync(id);
    }
}
