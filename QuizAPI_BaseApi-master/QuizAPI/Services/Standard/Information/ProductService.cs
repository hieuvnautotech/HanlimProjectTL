using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Base;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common.Standard.Information
{
    public interface IProductService
    {
        Task<ResponseModel<IEnumerable<ProductDto>?>> GetAll(ProductDto pageInfo);
        Task<string> Create(ProductDto model);
        Task<ResponseModel<ProductDto?>> GetById(long id);
        Task<string> Modify(ProductDto model);
        Task<string> Delete(ProductDto model);
        Task<ResponseModel<IEnumerable<ProductDto>?>> GetActive(ProductDto model);
    }
    [ScopedRegistration]
    public class ProductService : IProductService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ProductService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<ResponseModel<IEnumerable<ProductDto>?>> GetAll(ProductDto model)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<ProductDto>?>();
                string proc = "Usp_Product_GetAll"; var param = new DynamicParameters();
                param.Add("@page", model.page);
                param.Add("@pageSize", model.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@MaterialCode", model.MaterialCode);
                param.Add("@Description", model.Description);
                param.Add("@Model", model.Model);
                param.Add("@ProductType",model.ProductType);
                param.Add("@showDelete", model.showDelete);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<ProductDto>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
                return returnData;
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public async Task<string> Create(ProductDto model)
        {
            try
            {
                string proc = "Usp_Product_Create";
                var param = new DynamicParameters();
                param.Add("@MaterialId", model.MaterialId);
                param.Add("@MaterialCode", model.MaterialCode?.ToUpper());
                param.Add("@ProductType", model.ProductType);
                param.Add("@Model", model.Model);
                param.Add("@Inch", model.Inch);
                param.Add("@Description", model.Description);
                param.Add("@createdBy", model.createdBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

                return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
            }
            catch (Exception e)
            {

                throw;
            }
          
        }
        public async Task<ResponseModel<ProductDto?>> GetById(long id)
        {
            try
            {
                var returnData = new ResponseModel<ProductDto?>();
                string proc = "Usp_Product_GetById";
                var param = new DynamicParameters();
                param.Add("@MaterialId", id);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<ProductDto?>(proc, param);
                returnData.Data = data.FirstOrDefault();
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
                return returnData;
            }
            catch (Exception e)
            {

                throw;
            }
          
        }
        public async Task<string> Modify(ProductDto model)
        {
            try
            {
                string proc = "Usp_Product_Modify";
                var param = new DynamicParameters();
                param.Add("@MaterialId", model.MaterialId);
                param.Add("@MaterialCode", model.MaterialCode?.ToUpper());
                param.Add("@Inch", model.Inch);
                param.Add("@Description", model.Description);
                param.Add("@ProductType", model.ProductType);
                param.Add("@Model", model.Model);
                param.Add("@modifiedBy", model.modifiedBy);
                param.Add("@row_version", model.row_version);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

                return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
            }
            catch (Exception e)
            {

                throw;
            }
     
        }
        public async Task<string> Delete(ProductDto model)
        {
            string proc = "Usp_Product_Delete";
            var param = new DynamicParameters();
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@row_version", model.row_version);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<ProductDto>?>> GetActive(ProductDto model)
        {
            var returnData = new ResponseModel<IEnumerable<ProductDto>?>();
            var proc = $"Usp_Product_GetActive";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<ProductDto>(proc);

            if (!data.Any())
            {
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                returnData.HttpResponseCode = 204;
            }
            else
            {
                returnData.Data = data;
            }

            return returnData;
        }
    }
}
