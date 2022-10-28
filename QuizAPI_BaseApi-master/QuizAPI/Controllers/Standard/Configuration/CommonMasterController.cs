using Microsoft.AspNetCore.Authorization;
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
    //[RoleAuthorization(RoleConst.ROOT)]
    public class CommonMasterController : ControllerBase
    {
        private readonly ICommonMasterService _commonMasterService;
        private readonly IJwtService _jwtService;

        public CommonMasterController(ICommonMasterService commonMasterService, IJwtService jwtService)
        {
            _commonMasterService = commonMasterService;
            _jwtService = jwtService;
        }

        [HttpGet("get-all")]
        [PermissionAuthorization(PermissionConst.COMMONMASTER_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string? keyWord, bool showDelete = true)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var roleName = _jwtService.GetRolename(token);
            var returnData = await _commonMasterService.GetAll(pageInfo, roleName,keyWord, showDelete);
            return Ok(returnData);
        }
        [HttpPost("create-commonmaster")]
        [PermissionAuthorization(PermissionConst.COMMONMASTER_CREATE)]
        public async Task<IActionResult> Create(CommonMasterDto model)
        {
            model.commonMasterId = AutoId.AutoGenerate();
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            var result = await _commonMasterService.Create(model);
            var returnData = new ResponseModel<CommonMasterDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SUCCESS:
                    returnData = await _commonMasterService.GetById(model.commonMasterId);
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }

        [HttpPut("modify-commonmaster")]
        [PermissionAuthorization(PermissionConst.COMMONMASTER_UPDATE)]
        public async Task<IActionResult> Modify(CommonMasterDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _commonMasterService.Modify(model);

            var returnData = new ResponseModel<CommonMasterDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                   returnData = await _commonMasterService.GetById(model.commonMasterId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return Ok(returnData);
        }
        [HttpDelete("delete-commonmaster")]
        //[PermissionAuthorization(PermissionConst.USER_UPDATE)]
        public async Task<IActionResult> Delete([FromBody] CommonMasterDto model)
        {
            try
            {
                var result = await _commonMasterService.Delete(model.commonMasterId, model.row_version);
                var returnData = new ResponseModel<CommonMasterDto?>();
                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.COMMONDETAIL_EXISTED:
                        returnData.ResponseMessage = result;
                        returnData.HttpResponseCode = 300;
                        break;
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
            catch (Exception e)
            {

                throw;
            }
          
        }
        
    }
}
