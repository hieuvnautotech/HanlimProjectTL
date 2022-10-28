using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.Services.Common;
using QuizAPI.Services.PO;
using QuizAPI.Services;
using QuizAPI.Services.MMS;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;

namespace QuizAPI.Controllers.MMS
{
    [Route("api/work-order")]
    [ApiController]
    public class WorkOrderController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IWorkOrderService _workOrderService;
        private readonly ICustomService _customService;
        public WorkOrderController(IJwtService jwtService, IWorkOrderService workOrderService, ICustomService customService)
        {
            _jwtService = jwtService;
            _workOrderService = workOrderService;
            _customService = customService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.WORKORDER_READ)]
        public async Task<IActionResult> Get([FromQuery] WorkOrderDto model)
        {
            var res = await _workOrderService.Get(model);
            return Ok(res);
        }

        [HttpPost("create-wo")]
        [PermissionAuthorization(PermissionConst.WORKORDER_CREATE)]
        public async Task<IActionResult> Create([FromBody] WorkOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.WoId = AutoId.AutoGenerate();

            var returnData = new ResponseModel<WorkOrderDto?>();
            var validator = new WorkOrderValidator();
            var validateResults = validator.Validate(model);

            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return Ok(returnData);
            }

            var result = await _workOrderService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _workOrderService.GetById(model.WoId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-wo")]
        [PermissionAuthorization(PermissionConst.WORKORDER_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] WorkOrderDto model)
        {
            var returnData = new ResponseModel<WorkOrderDto?>();

            var validator = new WorkOrderValidator();
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

            var result = await _workOrderService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _workOrderService.GetById(model.WoId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("delete-reuse-wo")]
        [PermissionAuthorization(PermissionConst.WORKORDER_DELETE)]
        public async Task<IActionResult> DeleteReuse([FromBody] WorkOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _workOrderService.DeleteReuse(model);

            var returnData = new ResponseModel<DeliveryOrderDto?>();
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

        [HttpGet("get-forecast-po-master")]
        [PermissionAuthorization(PermissionConst.WORKORDER_READ)]
        public async Task<IActionResult> GetForecastPOMasters()
        {
            string Column = "FPoMasterId, FPoMasterCode";
            string Table = "ForecastPOMaster";
            string Where = "isActived = 1";
            string Order = "FPoMasterId";

            return Ok(await _customService.GetForSelect<ForecastPOMasterDto>(Column, Table, Where, Order));
        }

        [HttpGet("get-lines")]
        [PermissionAuthorization(PermissionConst.WORKORDER_READ)]
        public async Task<IActionResult> GetLines()
        {
            string Column = "LineId, LineName";
            string Table = "Line";
            string Where = "isActived = 1";
            string Order = "LineName";

            return Ok(await _customService.GetForSelect<LineDto>(Column, Table, Where, Order));
        }

        [HttpGet("get-products-by-po-master")]
        [PermissionAuthorization(PermissionConst.WORKORDER_READ)]
        public async Task<IActionResult> GetProductsByPOMaster(long fPoMasterId)
        {
            return Ok(await _workOrderService.GetProductsByForecastPOMaster(fPoMasterId));
        }
    }
}
