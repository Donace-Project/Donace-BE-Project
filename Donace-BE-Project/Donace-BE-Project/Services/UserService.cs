using AutoMapper;
using Donace_BE_Project.Entities;
using Donace_BE_Project.Interfaces;
using Donace_BE_Project.Models.User;
using EntityFramework.Repository;

namespace Donace_BE_Project.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository repo,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserModel> RegisterAsync(string email)
    {
        User user = new()
        {
            Email = email,
        };

        var created = await _repo.CreateAsync(user);
        await _unitOfWork.SaveChangeAsync();

        return _mapper.Map<UserModel>(created);
    }

    public async Task<UserModel> GetProfileAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        return _mapper.Map<UserModel>(user);
    }
}
