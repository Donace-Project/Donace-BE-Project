using AutoMapper;
using Donace_BE_Project.Interfaces;
using Donace_BE_Project.Models.User;
using EntityFramework.Repository;

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
