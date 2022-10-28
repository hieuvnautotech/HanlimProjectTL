using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.CustomAttributes;
using QuizAPI.Services.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using FluentValidation;
using QuizAPI.Services.PO;
using QuizAPI.Services.PurchaseOrder;

namespace QuizAPI.Controllers.PurchaseOrder
{
    [Route("api/forecast-po")]
    [ApiController]
    public class ForecastPOController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IForecastPOService _forecastPOService;
       
        public ForecastPOController(IJwtService jwtService, IForecastPOService forecastPOService)
        {
            _jwtService = jwtService;
            _forecastPOService = forecastPOService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.FORECASTPO_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, int keyWordWeekStart, int keyWordWeekEnd, int keyWordYear, bool showDelete = true)
        {
            var returnData = await _forecastPOService.GetAll(pageInfo, keyWord, keyWordWeekStart, keyWordWeekEnd, keyWordYear, showDelete);
            return Ok(returnData);
        }

        [HttpPost("create-forecast")]
        [PermissionAuthorization(PermissionConst.FORECASTPO_CREATE)]
        public async Task<IActionResult> Create([FromBody] ForecastPODto model)
        {
            var returnData = new ResponseModel<ForecastPODto?>();
            var validator = new ForecastValidator();
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
            model.FPOId = AutoId.AutoGenerate();

            var result = await _forecastPOService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _forecastPOService.GetById(model.FPOId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-forecast")]
        [PermissionAuthorization(PermissionConst.FORECASTPO_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] ForecastPODto model)
        {
            var returnData = new ResponseModel<ForecastPODto?>();

            var validator = new ForecastValidator();
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

            var result = await _forecastPOService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _forecastPOService.GetById(model.FPOId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpDelete("delete-forecast")]
        [PermissionAuthorization(PermissionConst.FORECASTPO_DELETE)]
        public async Task<IActionResult> Delete([FromBody] ForecastPODto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);
            var result = await _forecastPOService.Delete(model.FPOId, model.row_version, createdBy);

            return Ok(result);
        }

        [HttpGet("get-select-material")]
        public async Task<IActionResult> GetMaterialModel()
        {
            return Ok(await _forecastPOService.SelectBoxMaterial());
        }

        [HttpGet("get-select-line")]
        public async Task<IActionResult> GetLineModel()
        {
            return Ok(await _forecastPOService.SelectBoxLine());
        }

        [HttpGet("get-select-year")]
        public async Task<IActionResult> GetYearModel()
        {
            return Ok(await _forecastPOService.SelectBoxYear());
        }
    }
}
