using AutoMapper;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository repo,
        IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<UserModel> GetProfileAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        return _mapper.Map<UserModel>(user);
    }
}
