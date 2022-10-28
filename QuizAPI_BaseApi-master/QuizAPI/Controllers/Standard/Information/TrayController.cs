using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;
using QuizAPI.Services.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrayController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ITrayService _trayService;
        private readonly ICommonMasterService _commonMasterService;

        public TrayController(IJwtService jwtService, ITrayService trayService, ICommonMasterService commonMasterService)
        {
            _jwtService = jwtService;
            _trayService = trayService;
            _commonMasterService = commonMasterService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.TRAY_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, long? TrayType, bool showDelete = true)
        {
            var returnData = await _trayService.GetAll(pageInfo, keyWord, TrayType, showDelete);
            return Ok(returnData);
        }

        [HttpPost("create")]
        [PermissionAuthorization(PermissionConst.TRAY_CREATE)]
        public async Task<IActionResult> Create([FromBody] TrayDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.TrayId = AutoId.AutoGenerate();

            var result = await _trayService.Create(model);

            return Ok(result);
        }

        [HttpPut("update")]
        [PermissionAuthorization(PermissionConst.TRAY_UPDATE)]
        public async Task<IActionResult> Update([FromBody] TrayDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);

            var result = await _trayService.Modify(model);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [PermissionAuthorization(PermissionConst.TRAY_DELETE)]
        public async Task<IActionResult> Delete([FromBody] TrayDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);
            var result = await _trayService.Delete(model.TrayId, model.row_version, createdBy);

            return Ok(result);
        }

        [HttpGet("get-tray-type")]
        public async Task<IActionResult> GetTrayType()
        {
            return Ok(await _commonMasterService.GetForSelect("TRAY TYPE"));
        }
    }
}
