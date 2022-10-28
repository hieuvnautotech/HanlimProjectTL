using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services;
using QuizAPI.Services.Common;
using QuizAPI.Services.PO;
using System.Data.Common;

namespace QuizAPI.Controllers.PurchaseOrder
{
    [Route("api/delivery-order")]
    [ApiController]
    public class DeliveryOrderController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IDeliveryOrderService _deliveryOrderService;
        private readonly ICustomService _customService;

        public DeliveryOrderController(IJwtService jwtService, IDeliveryOrderService deliveryOrderService, ICustomService customService)
        {
            _jwtService = jwtService;
            _deliveryOrderService = deliveryOrderService;
            _customService = customService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.DELIVERYORDER_READ)]
        public async Task<IActionResult> Get([FromQuery] DeliveryOrderDto model)
        {
            var res = await _deliveryOrderService.Get(model);
            return Ok(res);
        }

        [HttpPost("create-do")]
        [PermissionAuthorization(PermissionConst.DELIVERYORDER_CREATE)]
        public async Task<IActionResult> Create([FromBody] DeliveryOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.DoId = AutoId.AutoGenerate();

            var returnData = new ResponseModel<DeliveryOrderDto?>();
            var validator = new DeliveryOrderValidator();
            var validateResults = validator.Validate(model);

            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return Ok(returnData);
            }

            var result = await _deliveryOrderService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _deliveryOrderService.GetById(model.DoId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-do")]
        [PermissionAuthorization(PermissionConst.DELIVERYORDER_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] DeliveryOrderDto model)
        {
            var returnData = new ResponseModel<DeliveryOrderDto?>();

            var validator = new DeliveryOrderValidator();
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

            var result = await _deliveryOrderService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _deliveryOrderService.GetById(model.DoId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("delete-reuse-do")]
        [PermissionAuthorization(PermissionConst.DELIVERYORDER_DELETE)]
        public async Task<IActionResult> DeleteReuse([FromBody] DeliveryOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _deliveryOrderService.DeleteReuse(model);

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

        [HttpGet("get-purchase-orders")]
        [PermissionAuthorization(PermissionConst.DELIVERYORDER_READ)]
        public async Task<IActionResult> GetPurchaseOrders()
        {
            string Column = "PoId, PoCode";
            string Table = "PurchaseOrder";
            string Where = "isActived = 1";
            string Order = "PoId";

            return Ok(await _customService.GetForSelect<PurchaseOrderDto>(Column, Table, Where, Order));
        }

        [HttpGet("get-products-by-po")]
        [PermissionAuthorization(PermissionConst.DELIVERYORDER_READ)]
        public async Task<IActionResult> GetProductsByPO(long poId)
        {
            return Ok(await _deliveryOrderService.GetProductsByPO(poId));
        }
    }
}
