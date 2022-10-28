using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services;
using QuizAPI.Services.Common;

namespace QuizAPI.Controllers.Standard.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAll]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _DocumentService;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IJwtService _jwtService;
        private readonly ICustomService _customService;
        private readonly ICommonMasterService _commonMasterService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public DocumentController(IWebHostEnvironment webHostEnvironment, ICommonMasterService commonMasterService, IDocumentService DocumentService, IMemoryCache memoryCache, IUserAuthorizationService userAuthorizationService, IJwtService jwtService, ICustomService customService)
        {
            _DocumentService = DocumentService;
            _memoryCache = memoryCache;
            _userAuthorizationService = userAuthorizationService;
            _jwtService = jwtService;
            _customService = customService;
            _commonMasterService = commonMasterService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.DOCUMENT_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, string language, bool showDelete = true)
        {
            var data = await _DocumentService.GetAll(pageInfo, keyWord, language, showDelete);
            return Ok(data);
        }

        [HttpPost("create-Document")]
        [PermissionAuthorization(PermissionConst.DOCUMENT_CREATE)]
        public async Task<IActionResult> Create([FromForm] DocumentDto model)
        {
            //xử lí lưu file apk
            if(model.file != null)
            {
                var webPath =  _webHostEnvironment.WebRootPath;
                string folder_path = Path.Combine($"{webPath}/Document/{model.language}");
                if (!Directory.Exists(folder_path))
                {
                    Directory.CreateDirectory(folder_path);
                }

                string ext = Path.GetExtension(model.file.FileName);
                string filename = DateTime.Now.ToString("yyyyMMddhhmmss") +"-"+ model.menuComponent + ext;
                using (var stream = System.IO.File.Create(Path.Combine(folder_path, filename)))
                {
                    await model.file.CopyToAsync(stream);
                }
                model.urlFile = filename;
            }    

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.createdBy = _jwtService.GetUserIdFromToken(token);
            model.documentId = AutoId.AutoGenerate();
            var result = await _DocumentService.Create(model);

            return Ok(result);
        }

        [HttpPut("modify-Document")]
        [PermissionAuthorization(PermissionConst.DOCUMENT_UPDATE)]
        public async Task<IActionResult> Modify([FromForm] DocumentDto model)
        {
            if (model.file != null)
            {
                var webPath = _webHostEnvironment.WebRootPath;
                string folder_path = Path.Combine($"{webPath}/Document/{model.language}");
                if (!Directory.Exists(folder_path))
                {
                    Directory.CreateDirectory(folder_path);
                }

                string ext = Path.GetExtension(model.file.FileName);
                string filename = DateTime.Now.ToString("yyyyMMddhhmmss") + "-" + model.menuComponent + ext;
                using (var stream = System.IO.File.Create(Path.Combine(folder_path, filename)))
                {
                    await model.file.CopyToAsync(stream);
                }
                model.urlFile = filename;
            }

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.modifiedBy = _jwtService.GetUserIdFromToken(token);
            var result = await _DocumentService.Modify(model);

            return Ok(result);
        }

        [HttpGet("download/{menuComponent}/{language}")]
        public async Task<IActionResult> DownloadFile(string menuComponent, string language)
        {
            try
            {
                var result = await _DocumentService.GetByComponent(menuComponent, language);
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpDelete("delete-Document")]
        [PermissionAuthorization(PermissionConst.DOCUMENT_DELETE)]
        public async Task<IActionResult> Delete(DocumentDto model)
        {
            var result = await _DocumentService.Delete(model);

            var returnData = new ResponseModel<UserDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    var userAuthorization = _userAuthorizationService.GetUserAuthorization();
                    _memoryCache.Remove("userAuthorization");
                    _memoryCache.Set("userAuthorization", userAuthorization.Result);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpGet("get-menu-component")]
        public async Task<IActionResult> GetMenuComponent()
        {
            string Column = "menuComponent, menuName ";
            string Table = "sysTbl_Menu";
            string Where = "isnull(menuComponent, '') <> ''";

            return Ok(await _customService.GetForSelect<dynamic>(Column, Table, Where, ""));
        }

        [HttpGet("get-language")]
        public async Task<IActionResult> GetLanguage()
        {
            return Ok(await _commonMasterService.GetForSelect("LANGUAGE"));
        }
    }
}
