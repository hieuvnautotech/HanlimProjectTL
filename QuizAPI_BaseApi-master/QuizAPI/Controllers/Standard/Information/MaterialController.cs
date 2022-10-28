using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;
using QuizAPI.Services.Common.Standard.Information;
using QuizAPI.Services.Standard.Information;

namespace QuizAPI.Controllers.Standard.Information
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _MaterialService;
        private readonly ICommonMasterService _commonMasterService;
        private readonly ISupplierService _supplierService;
        private readonly IJwtService _jwtService;

        public MaterialController(ISupplierService supplierService,IMaterialService MaterialService, IJwtService jwtService, ICommonMasterService commonMasterService)
        {
            _MaterialService = MaterialService;
            _jwtService = jwtService;
            _commonMasterService = commonMasterService;
            _supplierService = supplierService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.MATERIAL_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, long? MaterialType, long? Unit, long? SupplierId, bool showDelete = true)
        {
            var returnData = await _MaterialService.GetAll(pageInfo, keyWord, MaterialType, Unit, SupplierId, showDelete);
            return Ok(returnData);
        }

        [HttpPost("create")]
        [PermissionAuthorization(PermissionConst.MATERIAL_CREATE)]
        public async Task<IActionResult> Create([FromBody] MaterialDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.MaterialId = AutoId.AutoGenerate();

            var result = await _MaterialService.Create(model);

            return Ok(result);
        }

        [HttpPut("update")]
        [PermissionAuthorization(PermissionConst.MATERIAL_UPDATE)]
        public async Task<IActionResult> Update([FromBody] MaterialDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);

            var result = await _MaterialService.Modify(model);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [PermissionAuthorization(PermissionConst.MATERIAL_DELETE)]
        public async Task<IActionResult> Delete([FromBody] MaterialDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            var createdBy = long.Parse(userId);
            var result = await _MaterialService.Delete(model.MaterialId, model.row_version, createdBy);

            return Ok(result);
        }

        [HttpGet("get-material-type")]
        public async Task<IActionResult> GetMaterialType()
        {
            var list = await _commonMasterService.GetForSelect("MATERIAL TYPE");
            list.Data = list.Data.Where(x => x.commonDetailName != "FINISH GOOD");
            return Ok(list);
        }

        [HttpGet("get-unit")]
        public async Task<IActionResult> GetUnit()
        {
            return Ok(await _commonMasterService.GetForSelect("MATERIAL UNIT"));
        }

        [HttpGet("get-supplier")]
        public async Task<IActionResult> GetSupplier()
        {
            return Ok(await _supplierService.GetForSelect());
        }

        [HttpGet("get-supplier-by-id/{Id}")]
        public async Task<IActionResult> GetSupplierById(long Id)
        {
            return Ok(await _supplierService.GetByMaterial(Id));
        }
    }
}
