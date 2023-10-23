using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using Newtonsoft.Json;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace Donace_BE_Project.Services
{
    public class CommonService : ICommonService
    {
        private readonly IUserProvider _iUserProvider;
        private readonly ILogger<CommonService> _iLogger;
        public CommonService(IUserProvider iUserProvider,
                             ILogger<CommonService> logger)
        { 
            _iUserProvider = iUserProvider;
            _iLogger = logger;
        }
        public string UpLoadImageAsync(IFormFile file, Guid eventId)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var solutionDirectory = Directory.GetCurrentDirectory();
                    var imageDirectory = Path.Combine(solutionDirectory, "Images");

                    var imagePath = Path.Combine(imageDirectory, $"{eventId}");

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    return imagePath;
                }
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CacheService, "File null");
            }
            catch(Exception ex)
            {
                _iLogger.LogError($"CommonService.Exception: {ex.Message}", $"{JsonConvert.SerializeObject(file)}");
                throw new FriendlyException(ExceptionCode.Donace_BE_Project_Bad_Request_CommonService, ex.Message);
            }
        }
    }
}
