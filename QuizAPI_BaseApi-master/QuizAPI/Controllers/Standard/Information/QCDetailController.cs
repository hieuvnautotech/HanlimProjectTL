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
    [Route("api/QCDetail")]
    [ApiController]
    public class QCDetailController : ControllerBase
    {
        private readonly IQCDetailService _qcDetailService;
        private readonly IQCMasterService _qcMasterService;
        private readonly IStandardQCService _standardQCService;
        private readonly IProductService _productService;
        private readonly IJwtService _jwtService;
        public QCDetailController(IQCDetailService QCDetailService, IJwtService jwtService, IProductService productService, IQCMasterService qcMasterService, IStandardQCService standardQCService)
        {
            _qcDetailService = QCDetailService;
            _jwtService = jwtService;
            _productService = productService;
            _qcMasterService = qcMasterService;
            _standardQCService = standardQCService;
        }

        [HttpGet("get-all")]
        [PermissionAuthorization(PermissionConst.QCDETAIL_READ)]
        public async Task<IActionResult> GetAll([FromQuery] QCDetailDto item)
        {
            var returnData = await _qcDetailService.GetAll(item);
            return Ok(returnData);
        }
        [HttpGet("get-qcMaster-active")]
        [PermissionAuthorization(PermissionConst.QCDETAIL_READ)]
        public async Task<IActionResult> GetActive([FromQuery] QCMasterDto item)
        {
            var returnData = await _qcMasterService.GetActive(item);
            return Ok(returnData);
        }
        [HttpGet("get-StandardQC-active")]
        [PermissionAuthorization(PermissionConst.QCDETAIL_READ)]
        public async Task<IActionResult> GetActive([FromQuery] StandardQCDto item)
        {
            var returnData = await _standardQCService.GetActive(item);

            var list = new ResponseModel<IEnumerable<StandardQCDto>?>();

            IList<StandardQCDto> subjects = new List<StandardQCDto>();

            if (returnData != null)
            {
                foreach (var item1 in returnData.Data.ToList())
                {
                    var StandardQCOjb = new StandardQCDto()
                    {
                        QCCode = item1.QCCode + item1.Description == null || item1.Description == "" ? item1.QCCode : item1.QCCode + " - " + item1.Description,
                        QCId = item1.QCId

                    };
                    subjects.Add(StandardQCOjb);
                }
                list.Data = subjects;
                list.ResponseMessage = returnData.ResponseMessage;
                list.TotalRow = returnData.TotalRow;
            }
            return Ok(list);
        }
        [HttpPost("create-qcDetail")]
        [PermissionAuthorization(PermissionConst.QCDETAIL_CREATE)]
        public async Task<IActionResult> Create([FromBody] QCDetailDto model)
        {
            try
            {
                var returnData = new ResponseModel<QCDetailDto?>();

                var validator = new QCDetailValidator();
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
                model.QCDetailId = AutoId.AutoGenerate();

                var result = await _qcDetailService.Create(model);

                switch (result)
                {
                    case StaticReturnValue.SYSTEM_ERROR:
                        returnData.HttpResponseCode = 500;
                        break;
                    case StaticReturnValue.SUCCESS:
                        returnData = await _qcDetailService.GetById(model.QCDetailId);
                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }

                returnData.ResponseMessage = result;
                return Ok(returnData);
            }
            catch (Exception e)
            {

                throw;
            }
          
        }

        [HttpPut("modify-qcDetail")]
        [PermissionAuthorization(PermissionConst.QCDETAIL_UPDATE)]
        public async Task<IActionResult> Modify([FromBody] QCDetailDto model)
        {
            var returnData = new ResponseModel<QCDetailDto?>();

            var validator = new QCDetailValidator();
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

            var result = await _qcDetailService.Modify(model);

            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await _qcDetailService.GetById(model.QCDetailId);
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            returnData.ResponseMessage = result;
            return Ok(returnData);
        }
        [HttpDelete("delete-redo-qcDetail")]
        [PermissionAuthorization(PermissionConst.QCDETAIL_DELETE)]
        public async Task<IActionResult> Delete([FromBody] QCDetailDto model)
        {

            //var result = await _qcDetailService.Delete(model.QCDetailId, model.row_version);
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);

            var result = await _qcDetailService.Delete(model);

            var returnData = new ResponseModel<QCDetailDto?>();
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
