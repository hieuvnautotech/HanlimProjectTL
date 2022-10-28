using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services.Common;
using QuizAPI.Services.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILineService _lineService;

        public LineController(IJwtService jwtService, ILineService lineService)
        {
            _jwtService = jwtService;
            _lineService = lineService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.LINE_READ)]
        public async Task<IActionResult> Get([FromQuery] LineDto model)
        {
            return Ok(await _lineService.Get(model));
        }

        [HttpPost("create-line")]
        [PermissionAuthorization(PermissionConst.LINE_CREATE)]
        public async Task<IActionResult> Create([FromBody] LineDto model)
        {
            var returnData = new ResponseModel<LineDto?>();

            var validator = new LineValidator();
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
            model.LineId = AutoId.AutoGenerate();

            var result = await _lineService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _lineService.GetById(model.LineId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-line")]
        [PermissionAuthorization(PermissionConst.LINE_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] LineDto model)
        {
            var returnData = new ResponseModel<LineDto?>();

            var validator = new LineValidator();
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

            var result = await _lineService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _lineService.GetById(model.LineId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("delete-reuse-line")]
        [PermissionAuthorization(PermissionConst.LINE_DELETE)]
        public async Task<IActionResult> DeleteReuse([FromBody] LineDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _lineService.DeleteReuse(model);

            var returnData = new ResponseModel<LineDto?>();
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
