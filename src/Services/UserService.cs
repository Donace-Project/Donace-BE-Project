using AutoMapper;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Entities.User;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Repositories;
using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.User;
using Newtonsoft.Json;

namespace Donace_BE_Project.Services;

public class UserService : IUserService
{
    private readonly IUserProvider _userProvider;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _iLogger;
    private readonly IUserRepository _iUserRepository;
    private readonly IUnitOfWork _iUnitOfWork;
    public UserService(IUserProvider userProvider,
                       IMapper mapper,
                       ILogger<UserService> logger,
                       IUserRepository userRepository,
                       IUnitOfWork unitOfWork)
    {
        _userProvider = userProvider;
        _mapper = mapper;
        _iLogger = logger;
        _iUserRepository = userRepository;
        _iUnitOfWork = unitOfWork;
    }

    public async Task<ResponseModel<UpdateUserModel>> UpdateProfileAsync(UpdateUserModel model)
    {
        try
        {
            var email = _userProvider.GetEmailUser();
            var user = new User();
            var data = new User();

            user = await _iUserRepository.FindAsync(x => x.Email.Equals(email));
            if (user == null)
            {
                _iLogger.LogWarning($"UserService.Warning: Not Find email of user: {email}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_UserService, $"Not Find email of user: {email}");
            }
            data = _mapper.Map(model, user);

            _iUserRepository.Update(data);
            await _iUnitOfWork.SaveChangeCusAsync();

            return new ResponseModel<UpdateUserModel>(true, ResponseCode.Donace_BE_Project_CustomerService_Success, model, new());
        }
        catch(Exception ex )
        {
            _iLogger.LogError($"UserService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(model)}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_UserService, ex.Message);
        }
    }

    public async Task<ResponseModel<UserModel>> GetProfileAsync()
    {
        try
        {
            var email = _userProvider.GetEmailUser();

            var user = await _iUserRepository.FindAsync(x => x.Email.Equals(email));

            var result = _mapper.Map<UserModel>(user);

            return new ResponseModel<UserModel>(true, ResponseCode.Donace_BE_Project_CustomerService_Success, result, new PageInfoModel());
        }
        catch(Exception ex)
        {
            _iLogger.LogError($"UserService.Exception: {ex.Message}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_UserService, "Get profile fail");
        }
    }

    public async Task<ResponseModel<List<GetListUserInCalendarModel>>> ListUserAsync(List<Guid> Ids)
    {
        try
        {
            var strIds = Ids.Select(i => i.ToString());
            var listUser = await _iUserRepository.GetListAsync(x => strIds.Contains(x.Id) && x.IsDeleted == false);

            var data = _mapper.Map<List<GetListUserInCalendarModel>>(listUser);

            return new ResponseModel<List<GetListUserInCalendarModel>>(true, ResponseCode.Donace_BE_Project_CustomerService_Success, data, new(0,0,1));
        }
        catch (Exception ex)
        {
            _iLogger.LogError($"UserService.Exception: {ex.Message}");
            throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_UserService, "Get list user Fail");
        }

    }
}
