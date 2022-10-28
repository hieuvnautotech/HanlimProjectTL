using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;

namespace QuizAPI.Controllers.Standard.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IJwtService _jwtService;
        private readonly IPermissionService _permissionService;
        private readonly IMenuService _menuService;

        public RoleController(IMenuService menuService, IPermissionService permissionService, IRoleService roleService, IMemoryCache memoryCache, IUserAuthorizationService userAuthorizationService, IJwtService jwtService)
        {
            _roleService = roleService;
            _memoryCache = memoryCache;
            _userAuthorizationService = userAuthorizationService;
            _jwtService = jwtService;
            _permissionService = permissionService;
            _menuService =  menuService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.ROLE_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var roleName = _jwtService.GetRolename(token);
            return Ok(await _roleService.GetAll(pageInfo, roleName, keyWord));
        }

        [HttpGet("get-permission-by-role/{roleId}")]
        [PermissionAuthorization(PermissionConst.ROLE_READ)]
        public async Task<IActionResult> GetPermissionByRole(long roleId, [FromQuery] PageModel pageInfo, string keyWord)
        {
            var a = await _permissionService.GetByRole(pageInfo, roleId, keyWord);
            return Ok(a);
        }

        [HttpGet("get-menu-by-role/{roleId}")]
        [PermissionAuthorization(PermissionConst.ROLE_READ)]
        public async Task<IActionResult> GetMenuByRole(long roleId, [FromQuery] PageModel pageInfo, string keyWord)
        {
            var a = await _menuService.GetByRole(pageInfo, roleId, keyWord);
            return Ok(a);
        }

        [HttpPost("create-role")]
        [PermissionAuthorization(PermissionConst.ROLE_CREATE)]
        public async Task<IActionResult> Create(RoleDto model)
        {
            model.roleId = AutoId.AutoGenerate();
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.createdBy = _jwtService.GetUserIdFromToken(token);
            var result = await _roleService.Create(model);

            var returnData = new ResponseModel<RoleDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _roleService.GetById(model.roleId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpPut("modify-role")]
        [PermissionAuthorization(PermissionConst.ROLE_UPDATE)]
        public async Task<IActionResult> Modify(RoleDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.modifiedBy = _jwtService.GetUserIdFromToken(token);
            var result = await _roleService.Modify(model);

            var returnData = new ResponseModel<RoleDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _roleService.GetById(model.roleId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpDelete("delete-role/{roleId}")]
        [PermissionAuthorization(PermissionConst.ROLE_DELETE)]
        public async Task<IActionResult> Delete(long roleId)
        {
            var result = await _roleService.Delete(roleId);

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

        [HttpPut("set-permissions-for-role")]
        [PermissionAuthorization(PermissionConst.ROLE_UPDATE)]
        public async Task<IActionResult> SetPermission(RoleDto ROLE)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            ROLE.modifiedBy = _jwtService.GetUserIdFromToken(token);
            var result = await _roleService.SetPermission(ROLE);

            var returnData = new ResponseModel<RoleDto?>();
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

        [HttpGet("get-all-menu")]
        public async Task<IActionResult> GetAllMenu()
        {
            return Ok(await _menuService.GetForSelect());
        }

        [HttpGet("get-all-permission")]
        public async Task<IActionResult> GetAllPermission()
        {
            return Ok(await _permissionService.GetForSelect());
        }

        [HttpPost("add-permission")]
        [PermissionAuthorization(PermissionConst.ROLE_CREATE)]
        public async Task<IActionResult> AddPermission([FromBody] RoleDeleteDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.createdBy = _jwtService.GetUserIdFromToken(token);
            
            return Ok(await _roleService.SetPermissionForRole(model));
        }
        
        [HttpPost("add-menu")]
        [PermissionAuthorization(PermissionConst.ROLE_CREATE)]
        public async Task<IActionResult> AddMenu([FromBody] RoleDeleteDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.createdBy = _jwtService.GetUserIdFromToken(token);

            return Ok(await _roleService.SetMenuForRole(model));
        }

        [HttpPost("delete-permission")]
        [PermissionAuthorization(PermissionConst.ROLE_DELETE)]
        public async Task<IActionResult> DeletePermission([FromBody] RoleDeleteDto model)
        {
            return Ok(await _roleService.DeletePermissionForRole(model));
        }

        [HttpPost("delete-menu")]
        [PermissionAuthorization(PermissionConst.ROLE_DELETE)]
        public async Task<IActionResult> DeleteMenu([FromBody] RoleDeleteDto model)
        {
            return Ok(await _roleService.DeleteMenuForRole(model));
        }

    }
}
