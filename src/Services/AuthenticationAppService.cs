﻿using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.User;
using Donace_BE_Project.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Donace_BE_Project.Services;

public class AuthenticationAppService : IAuthenticationAppService
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;
    private readonly ICacheService _iCacheService;
    private readonly AppUserManager _userManager;

    private readonly JwtSetting _jwtSetting;
    private User? _user;

    public AuthenticationAppService(IUserRepository repo,
                       IMapper mapper,
                       IOptions<JwtSetting> jwtSetting,
                       AppUserManager userManager,
                       ICacheService iCacheService)
    {
        _repo = repo;
        _mapper = mapper;
        _userManager = userManager;

        _jwtSetting = jwtSetting.Value;
        _iCacheService = iCacheService;
    }

    public async Task<RegisterResponse> RegisterAsync(UserDto input)
    {
        var output = new RegisterResponse();
        User user = new()
        {
            Email = input.Email,
            UserName = input.Email,
        };

        output.Result = await _userManager.CreateAsync(user, input.Password);
        if (!output.Result.Succeeded) return output;

        _user = await _userManager.FindByEmailAsync(input.Email);
        output.Token = await CreateTokenAsync();
        output.User = _mapper.Map<User, UserModel>(_user);
        return output;
    }

    public async Task<LoginResponse> LoginAsync(UserDto input)
    {
        var output = new LoginResponse();


        _user = await _userManager.FindByEmailAsync(input.Email);
        if (_user is null)
        {
            throw new LoginException();
        }

        var resultLogin = await _userManager.CheckPasswordAsync(_user, input.Password);
        if (!resultLogin)
        {
            throw new LoginException();
        }

        output.Token = await CreateTokenAsync();
        output.User = _mapper.Map<User, UserModel>(_user);

        ///await _iCacheService.SetDataAsync($"{KeyCache.User}:{input.Email}:{input.Password}", _user);

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
        var key = Encoding.UTF8.GetBytes(_jwtSetting.Secret);
        var secret = new SymmetricSecurityKey(key);
        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, _user.Email),
            new Claim(ClaimTypes.NameIdentifier, _user.Id),
            new Claim(ClaimTypes.Name, _user.UserName)
        };
        var roles = await _userManager.GetRolesAsync(_user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken
        (
        issuer: _jwtSetting.Issuer,
        audience: _jwtSetting.Audience,
        claims: claims,
        expires: DateTime.MaxValue,
        signingCredentials: signingCredentials
        );
        return tokenOptions;
    }
    #endregion
}
