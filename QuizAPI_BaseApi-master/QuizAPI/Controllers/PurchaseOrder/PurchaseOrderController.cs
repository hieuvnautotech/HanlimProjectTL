using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;
using QuizAPI.Services.PO;
using QuizAPI.Services.Standard.Information;

namespace QuizAPI.Controllers.PurchaseOrder
{
    [Route("api/fixed-po")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PurchaseOrderController(IJwtService jwtService, IPurchaseOrderService purchaseOrderService, IWebHostEnvironment webHostEnvironment)
        {
            _jwtService = jwtService;
            _purchaseOrderService = purchaseOrderService;
            _webHostEnvironment = webHostEnvironment;
        }

        // Purchase Order
        [HttpGet]
        [PermissionAuthorization(PermissionConst.PURCHASEORDER_READ)]
        public async Task<IActionResult> Get([FromQuery] PurchaseOrderDto model)
        {
            return Ok(await _purchaseOrderService.Get(model));
        }

        [HttpPost("create")]
        [PermissionAuthorization(PermissionConst.PURCHASEORDER_CREATE)]
        public async Task<IActionResult> Create([FromBody] PurchaseOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.PoId = AutoId.AutoGenerate();

            var result = await _purchaseOrderService.Create(model);

            return Ok(result);
        }

        [HttpPut("update")]
        [PermissionAuthorization(PermissionConst.PURCHASEORDER_UPDATE)]
        public async Task<IActionResult> Update([FromBody] PurchaseOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);

            var result = await _purchaseOrderService.Modify(model);

            return Ok(result);
        }

        [HttpDelete("delete")]
        [PermissionAuthorization(PermissionConst.PURCHASEORDER_DELETE)]
        public async Task<IActionResult> Delete([FromBody] PurchaseOrderDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            var result = await _purchaseOrderService.Delete(model);

            return Ok(result);
        }

        // PO Detail
        [HttpGet("get-detail/{PoId}")]
        [PermissionAuthorization(PermissionConst.PODETAIL_READ)]
        public async Task<IActionResult> Get([FromQuery] BaseModel model, long PoId)
        {
            return Ok(await _purchaseOrderService.GetDetail(model, PoId));
        }

        [HttpPost("create-detail")]
        [PermissionAuthorization(PermissionConst.PODETAIL_CREATE)]
        public async Task<IActionResult> Create([FromBody] PODetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            model.PoDetailId = AutoId.AutoGenerate();

            var result = await _purchaseOrderService.CreateDetail(model);

            return Ok(result);
        }

        [HttpPut("update-detail")]
        [PermissionAuthorization(PermissionConst.PODETAIL_UPDATE)]
        public async Task<IActionResult> Update([FromBody] PODetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);

            var result = await _purchaseOrderService.ModifyDetail(model);

            return Ok(result);
        }

        [HttpDelete("delete-detail")]
        [PermissionAuthorization(PermissionConst.PODETAIL_DELETE)]
        public async Task<IActionResult> Delete([FromBody] PODetailDto model)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.createdBy = long.Parse(userId);
            var result = await _purchaseOrderService.DeleteDetail(model);

            return Ok(result);
        }

        [HttpGet("get-material/{PoId}")]
        public async Task<IActionResult> GetMaterial(long PoId)
        {
            return Ok(await _purchaseOrderService.GetMaterial(PoId));
        }

        [HttpGet("download-excel")]
        public IActionResult DownloadExcel([FromQuery] PurchaseOrderDto model)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string folderPath = Path.Combine(webRootPath, "TemplateReport/Excel");
            var filePath = Path.Combine(folderPath, "PoDetailReport.xlsx");
            var sheets = new Dictionary<string, object>();

            var PoDetail = _purchaseOrderService.GetForReport(model);
            sheets.Add("PoDetail", PoDetail.Result.Data);

            var memoryStream = new MemoryStream();
            MiniExcel.SaveAsByTemplate(memoryStream, filePath, sheets);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            { FileDownloadName = "PoDetailReport.xlsx" };
        }
    }
}
