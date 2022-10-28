using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services.Common;
using QuizAPI.Services.Standard.Information;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ISupplierService _supplierService;

        public SupplierController(IJwtService jwtService, ISupplierService supplierService)
        {
            _jwtService = jwtService;
            _supplierService = supplierService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.SUPPLIER_READ)]
        public async Task<IActionResult> Get([FromQuery] SupplierDto model)
        {
            return Ok(await _supplierService.Get(model));
        }

        [HttpPost("create-supplier")]
        [PermissionAuthorization(PermissionConst.SUPPLIER_CREATE)]
        public async Task<IActionResult> Create([FromBody] SupplierDto model)
        {
            var returnData = new ResponseModel<SupplierDto?>();

            var validator = new SupplierValidator();
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
            model.SupplierId = AutoId.AutoGenerate();

            var result = await _supplierService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _supplierService.GetById(model.SupplierId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-supplier")]
        [PermissionAuthorization(PermissionConst.SUPPLIER_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] SupplierDto model)
        {
            var returnData = new ResponseModel<SupplierDto?>();

            var validator = new SupplierValidator();
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

            var result = await _supplierService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _supplierService.GetById(model.SupplierId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("delete-reuse-supplier")]
        [PermissionAuthorization(PermissionConst.SUPPLIER_DELETE)]
        public async Task<IActionResult> DeleteReuse([FromBody] SupplierDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _supplierService.DeleteReuse(model);

            var returnData = new ResponseModel<SupplierDto?>();
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
