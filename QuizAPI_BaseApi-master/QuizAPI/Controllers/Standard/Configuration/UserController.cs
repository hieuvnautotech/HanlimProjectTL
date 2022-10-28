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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IJwtService _jwtService;
        private readonly IRoleService _roleService;

        public UserController(IUserService userService, IMemoryCache memoryCache, IUserAuthorizationService userAuthorizationService, IJwtService jwtService, IRoleService roleService)
        {
            _userService = userService;
            _memoryCache = memoryCache;
            _userAuthorizationService = userAuthorizationService;
            _jwtService = jwtService;
            _roleService = roleService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.USER_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string? keyword)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var userRole = await _userService.GetUserRole(long.Parse(userId));
            if (userRole.Contains(RoleConst.ROOT))
            {
                return Ok(await _userService.GetAll(pageInfo, keyword));
            }
            else
            {
                return Ok(await _userService.GetExceptRoot(pageInfo, keyword));
            }
        }
        

        [HttpGet("get-userrole")]
        [PermissionAuthorization(PermissionConst.USER_READ)]
        public async Task<IActionResult> GetUserRole(long userId)
        {
            var returnData = new ResponseModel<IEnumerable<string>?>
            {
                Data = await _userService.GetUserRole(userId)
            };

            if (!returnData.Data.Any())
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }

            return Ok(returnData);
        }

        [HttpGet("get-userpermission")]
        [PermissionAuthorization(PermissionConst.USER_READ)]
        public async Task<IActionResult> GetUserPermission(long userId)
        {
            var returnData = new ResponseModel<IEnumerable<string>?>
            {
                Data = await _userService.GetUserPermission(userId)
            };
            if (!returnData.Data.Any())
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }

            return Ok(returnData);
        }

        [HttpPost("create-user")]
        [PermissionAuthorization(PermissionConst.USER_CREATE)]
        public async Task<IActionResult> Create(UserDto userInfo)
        {
            userInfo.userId = AutoId.AutoGenerate();
            var result = await _userService.Create(userInfo);

            var returnData = new ResponseModel<UserDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    if(userInfo.Roles != null)
                        await _userService.SetUserInfoRole(userInfo);  
                    returnData = await _userService.GetByUserId(userInfo.userId);
                    returnData.ResponseMessage = StaticReturnValue.SUCCESS;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpPut("change-userpassword")]
        [AllowAll]
        public async Task<IActionResult> ChangeUserPassword(UserDto userInfo)
        {
            var returnData = new ResponseModel<UserDto?>();
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userName = _jwtService.GetUserNameFromToken(token);
            if (userName != userInfo.userName)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = StaticReturnValue.CHANGE_PASSWORD_NOT_ALLOWED;
            }

            else
            {
                var result = await _userService.ChangeUserPassword(userInfo);
                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.SYSTEM_ERROR:
                        returnData.HttpResponseCode = 500;
                        break;
                    case StaticReturnValue.SUCCESS:
                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }
            }

            return Ok(returnData);
        }

        [HttpPut("change-userpassword-by-root")]
        [PermissionAuthorization(PermissionConst.USER_UPDATE)]
        public async Task<IActionResult> ChangeUserPasswordByRoot(UserDto userInfo)
        {
            var returnData = new ResponseModel<UserDto?>();
            var result = await _userService.ChangeUserPasswordByRoot(userInfo);
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpPut("set-role-for-user")]
        [PermissionAuthorization(PermissionConst.USER_UPDATE)]
        public async Task<IActionResult> SetRoleForUser(UserDto userInfo)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            userInfo.modifiedBy = _jwtService.GetUserIdFromToken(token);
            var result = await _userService.SetUserInfoRole(userInfo);

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

        [HttpGet("check-loggedin")]
        [AllowAll]
        public async Task<IActionResult> CheckLoggedinUser()
        {
            var returnData = new ResponseModel<bool>();
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.GetUserIdFromToken(token);
            var check = false;
            if (userId > 0)
            {
                var user = await _userService.GetByUserId(userId);
                if (user.Data != null)
                {
                    check = user.Data.isActived ?? false;
                }

            }
            if (!check)
                returnData.HttpResponseCode = 401;

            returnData.Data = check;
            return Ok(returnData);
        }

        [HttpGet("get-role/{UserId}")]
        [PermissionAuthorization(PermissionConst.MENU_READ)]
        public async Task<IActionResult> GetByRoleByUser(long UserId)
        {
            return Ok(await _userService.GetRoleByUser(UserId));
        }

        [HttpDelete("delete-user/{userId}")]
        [PermissionAuthorization(PermissionConst.USER_DELETE)]
        public async Task<IActionResult> Delete(long userId)
        {

            var result = await _userService.Delete(userId);

            var returnData = new ResponseModel<UserDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpGet("get-all-role")]
        [PermissionAuthorization(PermissionConst.MENU_READ)]
        public async Task<IActionResult> GetAllRole()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var userRole = await _userService.GetUserRole(long.Parse(userId));
            return Ok(await _roleService.GetForSelect(userRole.ToList()));
        }
    }
}
