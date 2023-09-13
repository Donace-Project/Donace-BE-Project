using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Donace_BE_Project.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Donace_BE_Project.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _service;
    private readonly IConfiguration _configuration;

    public AuthenticationController(
        IAuthenticationService service,
        IConfiguration configuration
        )
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string email)
    {
        if (!string.IsNullOrEmpty(email))
        {
            // Create a list of claims (customize this as needed)
            var claims = new[]
            {
                    new Claim(ClaimTypes.Email, email),
            };

            // Create a JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30), // Token expiration time
                signingCredentials: creds
            );

            // Return the JWT token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                user = await _service.RegisterAsync(email)
            });
        }

        return Unauthorized("Authentication failed.");
    }

    [HttpGet("check-authentication")]
    [Authorize] // Requires authentication to access this endpoint
    public IActionResult CheckAuthentication()
    {
        // Get the authenticated user's claims
        var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;

        if (claimsIdentity != null)
        {
            // Extract user's information from claims (customize this as needed)
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var username = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // You can return user information or any other response here
            return Ok(new
            {
                UserId = userId,
                Username = username,
                Message = "Authentication successful!"
            });
        }

        return BadRequest("Authentication failed.");
    }

    /// <summary>
    /// Lấy một token tạm để check-authen.
    /// </summary>
    [HttpPost("get-token")]
    public IActionResult GetToken()
    {
        // Replace with your authentication logic (e.g., username and password validation)
        string username = "exampleuser"; // Replace with actual username
        string password = "examplepassword"; // Replace with actual password

        // Check username and password (this is a simplified example)
        if (username == "exampleuser" && password == "examplepassword")
        {
            // Create a list of claims (customize this as needed)
            var claims = new[]
            {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, "12345"), // Replace with a unique user identifier
                };

            // Create a JWT token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30), // Token expiration time
                signingCredentials: creds
            );

            // Return the JWT token
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        return Unauthorized("Authentication failed.");
    }
}
