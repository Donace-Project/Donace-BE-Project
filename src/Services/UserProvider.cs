using System.Security.Claims;
using Donace_BE_Project.Interfaces.Services;

namespace Donace_BE_Project.Services;

public class UserProvider : IUserProvider
{
    private readonly IHttpContextAccessor _context;

    public UserProvider(IHttpContextAccessor context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public string GetEmailUser()
    {
        var claim = _context?.HttpContext?.User.Claims
                   .FirstOrDefault(i => i.Type == ClaimTypes.Email);

        if (claim is null)
        {
            return string.Empty;
        }

        return claim.Value.ToString();
    }

    public Guid GetUserId()
    {
        var claim = _context?.HttpContext?.User.Claims
                   .FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);

        if (claim is null)
        {
            return Guid.Empty;
        }

        return new Guid(claim.Value);
    }
}
