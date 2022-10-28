using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;
using QuizAPI.Services.Common.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoldController : ControllerBase
    {
        private readonly IMoldService _MoldService;
        private readonly ICommonMasterService _commonMasterService;
        private readonly IJwtService _jwtService;

        public MoldController(IMoldService MoldService, IJwtService jwtService, ICommonMasterService commonMasterService)
        {
            _MoldService = MoldService;
            _jwtService = jwtService;
            _commonMasterService = commonMasterService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.MOLD_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, long? Model, long? MoldType, long? MachineType, bool showDelete = true)
        {
            var returnData = await _MoldService.GetAll(pageInfo, keyWord, Model, MoldType, MachineType, showDelete);
            return Ok(returnData);
        }

        [HttpPost("create")]
        [PermissionAuthorization(PermissionConst.MOLD_CREATE)]
        public async Task<IActionResult> Create([FromBody] MoldDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.MoldId = AutoId.AutoGenerate();
            model.ETAStatus = bool.Parse(model.ETAStatus1);

            var result = await _MoldService.Create(model);

            return Ok(result);
        }

        [HttpPut("update")]
        [PermissionAuthorization(PermissionConst.MOLD_UPDATE)]
        public async Task<IActionResult> Update([FromBody] MoldDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.ETAStatus = bool.Parse(model.ETAStatus1);

            var result = await _MoldService.Modify(model);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [PermissionAuthorization(PermissionConst.MOLD_DELETE)]
        public async Task<IActionResult> Delete([FromBody] MoldDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);
            var result = await _MoldService.Delete(model.MoldId, model.row_version, createdBy);

            return Ok(result);
        }

        [HttpGet("get-product-model")]
        public async Task<IActionResult> GetProductModel()
        {
            return Ok(await _commonMasterService.GetForSelect("PRODUCT MODEL"));
        }

        [HttpGet("get-product-type")]
        public async Task<IActionResult> GetProductType()
        {
            return Ok(await _commonMasterService.GetForSelect("PRODUCT TYPE"));
        }

        [HttpGet("get-machine-type")]
        public async Task<IActionResult> GetMachineType()
        {
            return Ok(await _commonMasterService.GetForSelect("MACHINE TYPE"));
        }
    }
}
