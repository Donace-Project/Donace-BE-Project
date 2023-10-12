using Donace_BE_Project.Models;
using Donace_BE_Project.Models.Calendar;
using Donace_BE_Project.Models.User;

namespace Donace_BE_Project.Interfaces.Services;
public interface IUserService
{
    Task<ResponseModel<UserModel>> GetProfileAsync();
    Task<ResponseModel<UpdateUserModel>> UpdateProfileAsync(UpdateUserModel model);
    Task<ResponseModel<List<GetListUserInCalendarModel>>> ListUserAsync(List<Guid> Ids);
}