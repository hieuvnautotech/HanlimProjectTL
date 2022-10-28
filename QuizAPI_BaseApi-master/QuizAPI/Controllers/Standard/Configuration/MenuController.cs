using Microsoft.AspNetCore.Http;
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
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IMemoryCache _memoryCache;

        public MenuController(IMenuService menuService, IUserService userService, IJwtService jwtService, IMemoryCache memoryCache)
        {
            _menuService = menuService;
            _userService = userService;
            _jwtService = jwtService;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.MENU_READ)]
        public async Task<IActionResult> Get([FromQuery] MenuDto model, string? keyWord)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var userRole = await _userService.GetUserRole(long.Parse(userId));
            if (userRole.Contains(RoleConst.ROOT))
            {
                return Ok(await _menuService.GetAll(model, keyWord));
            }
            else
            {
                return Ok(await _menuService.GetExceptRoot(model, keyWord));
            }
        }
        
        [HttpPost("create-menu")]
        [RoleAuthorization(RoleConst.ROOT)]
        [PermissionAuthorization(PermissionConst.MENU_CREATE)]
        public async Task<IActionResult> Create([FromBody] MenuDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.menuId = AutoId.AutoGenerate();
            if (model.parentId == 0)
            {
                model.parentId = null;
            }

            if (model.sortOrder == null)
            {
                model.sortOrder = 0;
            }

            var result = await _menuService.Create(model);

            var returnData = new ResponseModel<MenuDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _menuService.GetById(model.menuId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpPut("modify-menu")]
        [RoleAuthorization(RoleConst.ROOT)]
        [PermissionAuthorization(PermissionConst.MENU_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] MenuDto model)
        {
            if (model.parentId == 0)
            {
                model.parentId = null;
            }

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _menuService.Modify(model);


            var returnData = new ResponseModel<MenuDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _menuService.GetById(model.menuId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpDelete("delete-menu")]
        [RoleAuthorization(RoleConst.ROOT)]
        [PermissionAuthorization(PermissionConst.MENU_DELETE)]
        public async Task<IActionResult> Delete([FromBody] MenuDto model)
        {

            var result = await _menuService.Delete(model);

            var returnData = new ResponseModel<MenuDto?>();
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


        [HttpGet("get-by-level")]
        [PermissionAuthorization(PermissionConst.MENU_READ)]
        public async Task<IActionResult> GetByLevel(byte menuLevel)
        {
            return Ok(await _menuService.GetByLevel(menuLevel));
        }
    }
}
