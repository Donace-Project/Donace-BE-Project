using AutoMapper;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IUserProvider _userProvider;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repo,
                       IUserProvider userProvider,
                       IMapper mapper
        )
    {
        _repo = repo;
        _userProvider = userProvider;
        _mapper = mapper;
    }

    public async Task<UserModel> GetProfileAsync()
    {
        var userId = _userProvider.GetUserId();
        var user = await _repo.FindByIdAsync(userId);
        return _mapper.Map<UserModel>(user);
    }
}
