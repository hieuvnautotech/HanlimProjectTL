using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;

namespace QuizAPI.Controllers.Standard.Configuration
{
    [Route("api/[controller]")]
    [ApiController]
   // [RoleAuthorization(RoleConst.ROOT)]
    public class CommonDetailController : ControllerBase
    {
        private readonly ICommonDetailService _commonDetailService;
        private readonly IJwtService _jwtService;

        public CommonDetailController(ICommonDetailService commonDetailService, IJwtService jwtService)
        {
            _commonDetailService = commonDetailService;
            _jwtService = jwtService;
        }

        [HttpGet("getall-by-masterId")]
        [PermissionAuthorization(PermissionConst.COMMONDETAIL_READ)]
        public async Task<IActionResult> GetByCommonMasterId([FromQuery] PageModel pageInfo, long commonMasterId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            return Ok(await _commonDetailService.GetByCommonMasterId(pageInfo,long.Parse(userId),commonMasterId));
        }


        [HttpPost("create-commondetail")]
        [PermissionAuthorization(PermissionConst.COMMONDETAIL_CREATE)]
        public async Task<IActionResult> Create(CommonDetailDto model)
        {
            model.commonDetailId = AutoId.AutoGenerate();
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.createdBy = _jwtService.GetUserIdFromToken(token);
            var result = await _commonDetailService.Create(model);
            var returnData = new ResponseModel<CommonDetailDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SUCCESS:
                    returnData = await _commonDetailService.GetById(model.commonDetailId);
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

        [HttpPut("modify-commondetail")]
        [PermissionAuthorization(PermissionConst.COMMONDETAIL_UPDATE)]
        public async Task<IActionResult> Modify(CommonDetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            model.modifiedBy = _jwtService.GetUserIdFromToken(token);
            var result = await _commonDetailService.Modify(model);
            var returnData = new ResponseModel<CommonDetailDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SUCCESS:
                    returnData = await _commonDetailService.GetById(model.commonDetailId);
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
        [HttpDelete("delete-commondetail/{commonDetailId}")]
        //[PermissionAuthorization(PermissionConst.USER_UPDATE)]
        public async Task<IActionResult> Delete(long commonDetailId)
        {

            var result = await _commonDetailService.Delete(commonDetailId);

            var returnData = new ResponseModel<CommonMasterDto?>();
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
    }
}
