using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpGet("profile")]
    public async Task<ResponseModel<UserModel>> ProfileAsync()
    {
        return await _service.GetProfileAsync();
    }

    [HttpPut("update-profile")]
    public async Task<ResponseModel<UpdateUserModel>> UpdateProfileAsync(UpdateUserModel input)
    {
        return await _service.UpdateProfileAsync(input);
    }
}
