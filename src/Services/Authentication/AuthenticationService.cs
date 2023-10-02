using AutoMapper;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuthenticationService(
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
}
