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
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IJwtService _jwtService;
        private readonly ICommonMasterService _commonMasterService;
        public ProductController(IProductService productService, IJwtService jwtService, ICommonMasterService commonMasterService)
        {
            _productService = productService;
            _jwtService = jwtService;
            _commonMasterService = commonMasterService;
        }

        [HttpGet("get-all")]
        [PermissionAuthorization(PermissionConst.PRODUCT_READ)]
        public async Task<IActionResult> GetAll([FromQuery] ProductDto item)
        {
            var returnData = await _productService.GetAll(item);
            return Ok(returnData);
        }
        [HttpGet("get-product-model")]
        [PermissionAuthorization(PermissionConst.PRODUCT_READ)]
        public async Task<IActionResult> GetProductModel()
        {
            return Ok(await _commonMasterService.GetForSelect("PRODUCT MODEL"));
        }
        [HttpGet("get-product-type")]
        [PermissionAuthorization(PermissionConst.PRODUCT_READ)]
        public async Task<IActionResult> GetProductType()
        {
            return Ok(await _commonMasterService.GetForSelect("PRODUCT TYPE"));
        }
        [HttpPost("create-product")]
        [PermissionAuthorization(PermissionConst.PRODUCT_CREATE)]
        public async Task<IActionResult> Create(ProductDto model)
        {
            try
            {
                var returnData = new ResponseModel<ProductDto?>();

                var validator = new ProductValidator();
                var validateResults = validator.Validate(model);
                if (!validateResults.IsValid)
                {
                    returnData.HttpResponseCode = 400;
                    returnData.ResponseMessage = validateResults.Errors[0].ToString();
                    return Ok(returnData);
                }

                model.MaterialId = AutoId.AutoGenerate();
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var userId = _jwtService.ValidateToken(token);
                model.createdBy = long.Parse(userId);
                var result = await _productService.Create(model);
                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.SUCCESS:
                        returnData = await _productService.GetById(model.MaterialId);
                        returnData.ResponseMessage = result;
                        break;
                    case StaticReturnValue.SYSTEM_ERROR:
                        returnData.HttpResponseCode = 500;
                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }

                return Ok(returnData);
            }
            catch (Exception e)
            {

                throw;
            }
          
        }
        [HttpPut("modify-product")]
        [PermissionAuthorization(PermissionConst.PRODUCT_UPDATE)]
        public async Task<IActionResult> Modify(ProductDto model)
        {
            try
            {
                var returnData = new ResponseModel<ProductDto?>();

                var validator = new ProductValidator();
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
                var result = await _productService.Modify(model);

                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.SYSTEM_ERROR:
                        returnData.HttpResponseCode = 500;
                        break;
                    case StaticReturnValue.SUCCESS:
                        returnData = await _productService.GetById(model.MaterialId);
                        returnData.ResponseMessage = result;

                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }

                return Ok(returnData);
            }
            catch (Exception e)
            {

                throw;
            }
           
        }
        [HttpDelete("delete-product")]
        [PermissionAuthorization(PermissionConst.PRODUCT_DELETE)]
        public async Task<IActionResult> Delete([FromBody] ProductDto model)
        {

            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var userId = _jwtService.ValidateToken(token);
            model.modifiedBy = long.Parse(userId);

            var result = await _productService.Delete(model);

            var returnData = new ResponseModel<ProductDto?>();
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
