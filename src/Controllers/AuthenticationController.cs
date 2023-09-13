using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Donace_BE_Project.Interfaces;

using FirebaseAdmin.Auth;

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
    //private readonly IEmailSender _emailSender;

    public AuthenticationController(
        IAuthenticationService service,
        IConfiguration configuration,
        IEmailSender emailSender
        )
    {
        _service = service;
        _configuration = configuration;
        //_emailSender = emailSender;
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
    /// Lấy một token tạm để test check-authen thôi.
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

    [HttpPost("generate-jwt")]
    public IActionResult GenerateJwt(string email)
    {
        var user = FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email).Result;

        if (user != null)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Uid),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        return BadRequest("User not found or not authenticated.");
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string email, string password, string username)
    {
        try
        {
            var user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = email,
                Password = password,
                DisplayName = username
            });

            // Generate an email verification link
            var emailVerificationLink = await FirebaseAuth.DefaultInstance.GenerateEmailVerificationLinkAsync(user.Email);


            // I'm trying :_)))
            // Send the email verification link via Firebase Cloud Messaging (FCM)
            //var message = new Message
            //{
            //    Token = "1006138912563",
            //    Notification = new Notification
            //    {
            //        Title = "Verify Your Email",
            //        Body = "Click here to verify your email address.",
            //    },
            //    Data = new Dictionary<string, string>
            //{
            //    { "emailVerificationLink", emailVerificationLink },
            //},
            //};

            //await FirebaseMessaging.DefaultInstance.SendAsync(message);


            return Ok(new
            {
                Message = "User registered successfully. A verification email has been sent.",
                Link = emailVerificationLink,
                User = await _service.RegisterAsync(email)
            });
        }
        catch (FirebaseAuthException ex)
        {
            return BadRequest(new
            {
                Message = $"Registration failed: {ex.Message}"
            });
        }
    }
}
