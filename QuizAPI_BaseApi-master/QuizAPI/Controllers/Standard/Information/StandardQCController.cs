using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services.Common;
using QuizAPI.Services.Common.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/Standard-QC")]
    [ApiController]
    public class StandardQCController : ControllerBase
    {
        private readonly IStandardQCService _standardQCService;
        private readonly IJwtService _jwtService;
        public StandardQCController(IStandardQCService standardQCService, IJwtService jwtService)
        {
            _standardQCService = standardQCService;
            _jwtService = jwtService;
        }

        [HttpGet("get-all")]
        [PermissionAuthorization(PermissionConst.STANDARD_QC_READ)]
        public async Task<IActionResult> GetAll([FromQuery] StandardQCDto item)
        {
            var returnData = await _standardQCService.GetAll(item);
            return Ok(returnData);
        }
        [HttpPost("create-standardQC")]
        [PermissionAuthorization(PermissionConst.STANDARD_QC_CREATE)]
        public async Task<IActionResult> Create([FromBody] StandardQCDto model)
        {
            var returnData = new ResponseModel<StandardQCDto?>();

            var validator = new StandarQCValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return Ok(returnData);
            }

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.QCId = AutoId.AutoGenerate();

            var result = await _standardQCService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _standardQCService.GetById(model.QCId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-standardQC")]
        [PermissionAuthorization(PermissionConst.STANDARD_QC_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] StandardQCDto model)
        {
            var returnData = new ResponseModel<StandardQCDto?>();

            var validator = new StandarQCValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return Ok(returnData);
            }

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);

            var result = await _standardQCService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _standardQCService.GetById(model.QCId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }
        [HttpDelete("delete-redo-standardQC")]
        [PermissionAuthorization(PermissionConst.STANDARD_QC_DELETE)]
        public async Task<IActionResult> Delete([FromBody] StandardQCDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);

            var result = await _standardQCService.Delete(model);

            var returnData = new ResponseModel<StandardQCDto?>();
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
