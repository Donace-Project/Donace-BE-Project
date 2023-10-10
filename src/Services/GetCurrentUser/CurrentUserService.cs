using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;

namespace Donace_BE_Project.Services.GetCurrentUser
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _iHttpContextAccessor;
        private readonly ILogger<CurrentUserService> _iLogger;
        public CurrentUserService(IHttpContextAccessor iHttpContextAccessor,
                                  ILogger<CurrentUserService> logger)
        {

            _iHttpContextAccessor = iHttpContextAccessor;
            _iLogger = logger;
        }
        public string CurrentUserAsync(string claimType)
        {
            try
            {
                var context = _iHttpContextAccessor.HttpContext;

                if (context != null && context.Request.Headers.ContainsKey("Authorization"))
                {
                    string authorizationHeader = context.Request.Headers["Authorization"];

                    if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        string token = authorizationHeader.Substring("Bearer ".Length).Trim();

                        var tokenHandler = new JwtSecurityTokenHandler();

                        var tokenInfo = tokenHandler.ReadJwtToken(token);

                        var claim = tokenInfo.Claims.FirstOrDefault(c => c.Type == claimType);

                        return claim?.Value;
                    }
                }

                _iLogger.LogWarning($"CurrentUserService.Warning: Not Find Token");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Not_Found_CurrentUserService, "Not Find Token") ;
            }
            catch(Exception ex)
            {
                _iLogger.LogError($"CurrentUserService.Exception: {ex.Message}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CurrentUserService, ex.Message);
            }
        }
    }
}
