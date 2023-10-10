using Donace_BE_Project.Models.CurrentUser;
using Donace_BE_Project.Services.GetCurrentUser;

namespace Donace_BE_Project.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string CurrentUserAsync(string claim);

    }
}
