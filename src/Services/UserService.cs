using AutoMapper;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Donace_BE_Project.Services;

public class UserService : UserManager<User>, IUserService
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;
    private IConfiguration _configuration;
    private User? _user;

    public UserService(IUserRepository repo,
                       IMapper mapper,
                       IConfiguration configuration,

                       IUserStore<User> store,
                       IOptions<IdentityOptions> optionsAccessor,
                       IPasswordHasher<User> passwordHasher,
                       IEnumerable<IUserValidator<User>> userValidators,
                       IEnumerable<IPasswordValidator<User>> passwordValidators,
                       ILookupNormalizer keyNormalizer,
                       IdentityErrorDescriber errors,
                       IServiceProvider services,
                       ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
        _repo = repo;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<UserModel> GetProfileAsync(Guid id)
    {
        var user = await _repo.FindByIdAsync(id);
        return _mapper.Map<UserModel>(user);
    }

    public async Task<IdentityResult> RegisterAsync(UserDto input)
    {
        User user = new()
        {
            Email = input.Email,
        };

        return await CreateAsync(user, input.Password);
    }

    public async Task<LoginResponse> LoginAsync(UserDto input)
    {
        _user = await FindByEmailAsync(input.Email);
        if (_user is null)
        {
            throw new Exception();
        }

        var resultLogin = await CheckPasswordAsync(_user, input.Password);
        if (!resultLogin)
        {
            throw new Exception();
        }

        var output = new LoginResponse()
        {
            Token = await CreateTokenAsync()
        };

        return output;
    }

    public async Task<string> CreateTokenAsync()
    {
        if (_user is null)
        {
            throw new ArgumentNullException(nameof(_user));
        }

        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }

    #region Privite method 
    private SigningCredentials GetSigningCredentials()
    {
        var jwtConfig = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtConfig["Secret"]);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, _user.UserName)
        };
        var roles = await GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var jwtSettings = _configuration.GetSection("JwtConfig");
        var tokenOptions = new JwtSecurityToken
        (
        issuer: jwtSettings["validIssuer"],
        audience: jwtSettings["validAudience"],
        claims: claims,
        expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expiresIn"])),
        signingCredentials: signingCredentials
        );
        return tokenOptions;
    }
    #endregion
}
