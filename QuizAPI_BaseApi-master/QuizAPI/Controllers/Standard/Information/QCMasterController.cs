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
using System.Collections.Generic;
using System.Diagnostics;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/QCMaster")]
    [ApiController]
    public class QCMasterController : ControllerBase
    {
        private readonly IQCMasterService _qcMasterService;
        private readonly IProductService _productService;
        private readonly IJwtService _jwtService;
        private readonly IMaterialService _MaterialService;
        public QCMasterController(IQCMasterService qcMasterService, IJwtService jwtService, IProductService productService, IMaterialService MaterialService)
        {
            _qcMasterService = qcMasterService;
            _jwtService = jwtService;
            _productService = productService;
            _MaterialService = MaterialService;
        }

        [HttpGet("get-all")]
        [PermissionAuthorization(PermissionConst.QCMASTER_READ)]
        public async Task<IActionResult> GetAll([FromQuery] QCMasterDto item)
        {
            var returnData = await _qcMasterService.GetAll(item);
            return Ok(returnData);
        }
        [HttpGet("get-material-active")]
        [PermissionAuthorization(PermissionConst.QCMASTER_READ)]
        public async Task<IActionResult> GetActive([FromQuery] string qcType)
        {
            var returnData = await _qcMasterService.GetMaterialByQCType(qcType);
            return Ok(returnData);
        }

        [HttpGet("get-qc-type")]
        public async Task<IActionResult> GetQCType()
        {
            var list = await _qcMasterService.GetForSelect("QC TYPE");
            return Ok(list);
        }

        [HttpPost("create-qcMaster")]
        [PermissionAuthorization(PermissionConst.QCMASTER_CREATE)]
        public async Task<IActionResult> Create([FromBody] QCMasterDto model)
        {
            var returnData = new ResponseModel<QCMasterDto?>();

            var validator = new QCMasterValidator();
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
            model.QCMasterId = AutoId.AutoGenerate();

            var result = await _qcMasterService.Create(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _qcMasterService.GetById(model.QCMasterId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }

        [HttpPut("modify-qcMaster")]
        [PermissionAuthorization(PermissionConst.QCMASTER_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] QCMasterDto model)
        {
            var returnData = new ResponseModel<QCMasterDto?>();

            var validator = new QCMasterValidator();
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

            var result = await _qcMasterService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _qcMasterService.GetById(model.QCMasterId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }
        [HttpDelete("delete-redo-qcMaster")]
        [PermissionAuthorization(PermissionConst.QCMASTER_DELETE)]
        public async Task<IActionResult> Delete([FromBody] QCMasterDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _qcMasterService.Delete(model);

            var returnData = new ResponseModel<QCMasterDto?>();
            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.QCDETAIL_EXISTED:
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
    }
}
