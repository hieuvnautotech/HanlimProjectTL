using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Dtos;
using QuizAPI.Services.Common;
using QuizAPI.Services.Standard.Information;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http.Extensions;
using System.Reflection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Cors;

namespace QuizAPI.Controllers.Standard.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionAppController : ControllerBase
    {
        private readonly IVersionAppService _versionAppService;
        private readonly IJwtService _jwtService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VersionAppController(IVersionAppService versionAppService, IJwtService jwtService, IWebHostEnvironment webHostEnvironment)
        {
            _versionAppService = versionAppService;
            _jwtService = jwtService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("check-version-app")]
        public async Task<IActionResult> CheckVersionApp(int versionCode)
        {
            var returnData = await _versionAppService.CheckVersionApp(versionCode);
            return Ok(returnData);
        }

        [HttpGet("get-all-versionApp")]
        public async Task<IActionResult> GetAllVersionApp()
        {
            var returnData = await _versionAppService.GetAll();
            return Ok(returnData);
        }

        [HttpPost("update-versionApp")]
        public async Task<IActionResult> Update([FromForm] VersionAppDto input)
        {
            //xử lí lưu file apk
            var webPath = System.IO.Directory.GetCurrentDirectory();
            string folder_path = Path.Combine($"{webPath}/Upload/APK");
            if (!Directory.Exists(folder_path))
            {
                Directory.CreateDirectory(folder_path);
            }


            string[] files = Directory.GetFiles(folder_path);
            foreach (string item in files)
            {
                System.IO.File.Delete(item);
            }

            using (var stream = System.IO.File.Create(Path.Combine(folder_path, input.file.FileName)))
            {
                await input.file.CopyToAsync(stream);
            }
            input.name_file = input.file.FileName;

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            input.createdBy = long.Parse(userId);

            var returnData = new ResponseModel<VersionAppDto?>();
            var result = await _versionAppService.Modify(input);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _versionAppService.GetAll();
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
             return Ok(returnData);
        }

        [HttpGet("download-versionApp")]
        public async Task<IActionResult> DownloadFile()
        {
            try
            {
                //Get filename
                var returnData = new ResponseModel<VersionAppDto?>();
                returnData = await _versionAppService.GetAll();
                string fileName = returnData.Data.name_file;

                var webPath = System.IO.Directory.GetCurrentDirectory();
                string folder_path = Path.Combine($"{webPath}/Upload/APK/" , fileName);

                return PhysicalFile(folder_path, "application/vnd.android.package-archive", Path.GetFileName(folder_path));
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
