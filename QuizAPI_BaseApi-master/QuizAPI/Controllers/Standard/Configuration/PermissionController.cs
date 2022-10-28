using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;

namespace QuizAPI.Controllers.Standard.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IJwtService _jwtService;
        private readonly ICommonDetailService _commonDetailService;

        public PermissionController(IPermissionService permissionService, IMemoryCache memoryCache, IUserAuthorizationService userAuthorizationService, IJwtService jwtService, ICommonDetailService commonDetailService)
        {
            _permissionService = permissionService;
            _memoryCache = memoryCache;
            _userAuthorizationService = userAuthorizationService;
            _jwtService = jwtService;
            _commonDetailService = commonDetailService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.PERMISSION_READ)]
        public async Task<IActionResult> GetPermissions([FromQuery] PageModel pageInfo, string keyWord)
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var userId = _jwtService.ValidateToken(token);
                var returnData = await _permissionService.GetAll(pageInfo, long.Parse(userId), keyWord);
                return Ok(returnData);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpGet("get-permission-type")]
        [PermissionAuthorization(PermissionConst.COMMONDETAIL_READ)]
        public async Task<IActionResult> GetPermissionType()
        {
            try
            {
                var returnData = await _commonDetailService.GetPermissionType();
                return Ok(returnData);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost("create-permission")]
        [PermissionAuthorization(PermissionConst.PERMISSION_CREATE)]
        public async Task<IActionResult> Create(PermissionDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            var result = await _permissionService.Create(model);

            var returnData = new ResponseModel<PermissionDto?>();
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

        [HttpPut("modify-permission")]
        [PermissionAuthorization(PermissionConst.PERMISSION_UPDATE)]
        public async Task<IActionResult> Modify(PermissionDto model)
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var userId = _jwtService.ValidateToken(token);
                model.modifiedBy = long.Parse(userId);
                var result = await _permissionService.Modify(model);

                var returnData = new ResponseModel<PermissionDto?>();
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
                        returnData = await _permissionService.GetById(model.permissionId);
                        returnData.ResponseMessage = result;
                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }

                return Ok(returnData);
            }
            catch (Exception e)
            {

                throw;
            }
            
        }
    }
}
