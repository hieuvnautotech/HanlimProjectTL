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
    public class LocationController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly ILocationService _locationService;
        private readonly ICommonMasterService _commonMasterService;

        public LocationController(IJwtService jwtService, ILocationService locationService, ICommonMasterService commonMasterService)
        {
            _jwtService = jwtService;
            _locationService = locationService;
            _commonMasterService = commonMasterService;
        }

        [HttpGet]
        [PermissionAuthorization(PermissionConst.LOCATION_READ)]
        public async Task<IActionResult> GetAll([FromQuery] PageModel pageInfo, string keyWord, long? AreaId, bool showDelete = true)
        {
            var returnData = await _locationService.GetAll(pageInfo, keyWord, AreaId, showDelete);
            return Ok(returnData);
        }


        [HttpGet("get-area")]
        public async Task<IActionResult> GetArea()
        {
            return Ok(await _commonMasterService.GetForSelect("AREA"));
        }

        [HttpPost("create-location")]
        [PermissionAuthorization(PermissionConst.LOCATION_CREATE)]
        public async Task<IActionResult> Create([FromBody] LocationDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.LocationId = AutoId.AutoGenerate();

            var result = await _locationService.Create(model);

            return Ok(result);
        }

        [HttpPut("modify-location")]
        [PermissionAuthorization(PermissionConst.LOCATION_UPDATE)]
        public async Task<IActionResult> Update([FromBody] LocationDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);

            var result = await _locationService.Modify(model);

            return Ok(result);
        }

        [HttpDelete("delete-location")]
        [PermissionAuthorization(PermissionConst.LOCATION_DELETE)]
        public async Task<IActionResult> Delete([FromBody] LocationDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);
            var result = await _locationService.Delete(model);
            return Ok(result);
        }
    }
}
