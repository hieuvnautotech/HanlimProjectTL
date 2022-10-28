using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services.Common;
using QuizAPI.Services.Common.Standard.Information;
using QuizAPI.Services.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]

    public class BuyerController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IBuyerService _BuyerService;

        public BuyerController(IJwtService jwtService, IBuyerService BuyerService)
        {
            _jwtService = jwtService;
            _BuyerService = BuyerService;

        }
        [HttpGet("get-all")]
        [PermissionAuthorization(PermissionConst.BUYER_READ)]
        public async Task<IActionResult> Get([FromQuery] BuyerDto model)
        {
            return Ok(await _BuyerService.Get(model));
            //var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            //var userId = _jwtService.ValidateToken(token);
            //var returnData = await _StaffService.GetAll(pageInfo, long.Parse(userId));
            //return Ok(returnData);
        }

        [HttpPost("create-buyer")]
        [PermissionAuthorization(PermissionConst.BUYER_CREATE)]
        public async Task<IActionResult> Create([FromBody] BuyerDto model)
        {
            var returnData = new ResponseModel<BuyerDto?>();

            var validator = new BuyerValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                //foreach (var failure in validateResults.Errors)
                //{
                //    Console.WriteLine("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
                //}

                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return Ok(returnData);

            }

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.BuyerId = AutoId.AutoGenerate();

            var result = await _BuyerService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _BuyerService.GetById(model.BuyerId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-buyer")]
        [PermissionAuthorization(PermissionConst.BUYER_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] BuyerDto model)
        {
            var returnData = new ResponseModel<BuyerDto?>();

            var validator = new BuyerValidator();
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

            var result = await _BuyerService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _BuyerService.GetById(model.BuyerId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("delete-buyer")]
        [PermissionAuthorization(PermissionConst.BUYER_DELETE)]
        public async Task<IActionResult> Delete([FromBody] BuyerDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _BuyerService.Delete(model);

            var returnData = new ResponseModel<BuyerDto?>();
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
