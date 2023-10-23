using Donace_BE_Project.Interfaces.Services;
using Donace_BE_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Donace_BE_Project.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpPost("upload-file")]
        public async Task<string> UploadFile(IFormFile file)
        {
            return await _commonService.UpLoadImageAsync(file, Guid.NewGuid());
        }
    }
}
