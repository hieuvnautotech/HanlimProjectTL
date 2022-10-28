using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services;
using QuizAPI.Services.Common;
using QuizAPI.Services.Common.Standard.Information;
using QuizAPI.Services.Standard.Information;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class BomController : ControllerBase
    {
        private readonly IBomService _BomService;
        private readonly IJwtService _jwtService;
        private readonly ICustomService _customService;
        private readonly IMaterialService _MaterialService;

        public BomController(IMaterialService MaterialService, IBomService BomService, IJwtService jwtService,  ICustomService customService)
        {
            _BomService = BomService;
            _jwtService = jwtService;
            _customService = customService;
            _MaterialService = MaterialService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.BOM_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, long? MaterialId, bool showDelete = true)
        {
            var returnData = await _BomService.GetAll(pageInfo, keyWord, MaterialId, showDelete);
            return Ok(returnData);
        }

        [HttpGet("get-for-copy")]
        [PermissionAuthorization(PermissionConst.BOM_READ)]
        public async Task<IActionResult> GetForCopy([FromQuery] PageModel pageInfo, long BomId)
        {
            var returnData = await _BomService.GetForCopy(pageInfo, BomId);
            return Ok(returnData);
        }

        [HttpPost("create")]
        [PermissionAuthorization(PermissionConst.BOM_CREATE)]
        public async Task<IActionResult> Create([FromBody] BomDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.BomId = AutoId.AutoGenerate();

            var result = await _BomService.Create(model);

            return Ok(result);
        }

        [HttpPut("update")]
        [PermissionAuthorization(PermissionConst.BOM_UPDATE)]
        public async Task<IActionResult> Update([FromBody] BomDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);

            var result = await _BomService.Modify(model);

            return Ok(result);
        }

        [HttpPost("copy/{BomIdLV0}/{Version}")]
        [PermissionAuthorization(PermissionConst.BOM_CREATE)]
        public async Task<IActionResult> Copy([FromBody] List<BomDto> List, long BomIdLV0, string Version)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);
            var data = List.OrderBy(x => x.BomLevel).ToList();

            var result = await _BomService.Copy(data, Version, BomIdLV0, createdBy);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [PermissionAuthorization(PermissionConst.BOM_DELETE)]
        public async Task<IActionResult> Delete([FromBody] BomDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);

            var result = await _BomService.Delete(model.BomId, model.row_version, createdBy);

            return Ok(result);
        }

        [HttpGet("get-parent")]
        public async Task<IActionResult> GetParent(long BomId)
        {
            return Ok(await _BomService.GetParent(BomId));
        }

        //[HttpGet("get-product")]
        //public async Task<IActionResult> GetProduct()
        //{
        //    string Column = "ProductId, ProductCode";
        //    string Table = "Product";
        //    string Where = "isActived = 1";

        //    return Ok(await _customService.GetForSelect<ProductDto>(Column, Table, Where, ""));
        //}

        //[HttpGet("get-material")]
        //public async Task<IActionResult> GetMaterial()
        //{
        //    string Column = "MaterialId, MaterialCode";
        //    string Table = "Material";
        //    string Where = "isActived = 1";

        //    return Ok(await _customService.GetForSelect<MaterialDto>(Column, Table, Where, ""));
        //}

        [HttpGet("get-material/{BomLv}")]
        public async Task<IActionResult> GetMaterial(int BomLv, long? BomId)
        {
            return Ok(await _MaterialService.GetForBom(BomId, BomLv));
        }
    }
}
