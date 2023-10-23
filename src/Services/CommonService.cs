using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Donace_BE_Project.Constant;
using Donace_BE_Project.Exceptions;
using Donace_BE_Project.Interfaces.Services;
using Newtonsoft.Json;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;

namespace Donace_BE_Project.Services
{
    public class CommonService : ICommonService
    {
        private readonly IUserProvider _iUserProvider;
        private readonly ILogger<CommonService> _iLogger;
        private readonly IConfiguration _iConfiguration;
        public CommonService(IUserProvider iUserProvider,
                             ILogger<CommonService> logger,
                             IConfiguration configuration)
        { 
            _iUserProvider = iUserProvider;
            _iLogger = logger;
            _iConfiguration = configuration;
        }
        public async Task<string> UpLoadImageAsync(IFormFile file, Guid eventId)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(eventId.ToString(), stream),
                            PublicId = eventId.ToString(), 
                        };
                        var account = new Account
                        {
                            Cloud = _iConfiguration.GetSection("ImageServer").GetValue<string>("Name"),
                            ApiKey = _iConfiguration.GetSection("ImageServer").GetValue<string>("Key"),
                            ApiSecret = _iConfiguration.GetSection("ImageServer").GetValue<string>("SecretKey"),
                        };
                        var cloudinary = new Cloudinary(account);
                        var uploadResult = await cloudinary.UploadAsync(uploadParams);

                        return uploadResult.Url.ToString();
                    }
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
