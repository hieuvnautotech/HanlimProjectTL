using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services;
using QuizAPI.Services.Common;
using QuizAPI.Services.Common.Standard.Information;
using QuizAPI.Services.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class BomDetailController : ControllerBase
    {
        private readonly IBomDetailService _BomDetailService;
        private readonly IJwtService _jwtService;
        private readonly ICustomService _customService;

        public BomDetailController(ICustomService customService, IBomDetailService BomDetailService, IJwtService jwtService)
        {
            _BomDetailService = BomDetailService;
            _jwtService = jwtService;
            _customService = customService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.BOMDETAIL_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, long? BomId, long? MaterialId, bool showDelete = true)
        {
            var returnData = await _BomDetailService.GetByBomID(pageInfo, BomId, MaterialId, showDelete);
            return Ok(returnData);
        }

        [HttpPost("create")]
        [PermissionAuthorization(PermissionConst.BOMDETAIL_CREATE)]
        public async Task<IActionResult> Create([FromBody] BomDetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.BomDetailId = AutoId.AutoGenerate();

            var result = await _BomDetailService.Create(model);

            return Ok(result);
        }

        [HttpPut("update")]
        [PermissionAuthorization(PermissionConst.BOMDETAIL_UPDATE)]
        public async Task<IActionResult> Update([FromBody] BomDetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);

            var result = await _BomDetailService.Modify(model);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [PermissionAuthorization(PermissionConst.BOMDETAIL_DELETE)]
        public async Task<IActionResult> Delete([FromBody] BomDetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);

            var result = await _BomDetailService.Delete(model.BomDetailId, model.row_version, createdBy);

            return Ok(result);
        }

        [HttpGet("get-material")]
        public async Task<IActionResult> GetMaterial()
        {
            string Column = "MaterialId, MaterialCode, Unit ";
            string Table = "Material";
            string Where = "isActived = 1";

            return Ok(await _customService.GetForSelect<MaterialDto>(Column, Table, Where, ""));
        }
    }
}
