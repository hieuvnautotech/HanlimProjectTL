using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services.Base;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common.Standard.Information
{
    public interface IBomDetailService
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetByBomID(PageModel pageInfo, long? BomId, long? MaterialId, bool showDelete = true);
        Task<ResponseModel<BomDetailDto?>> GetById(long? BomDetailId);
        Task<ResponseModel<BomDetailDto?>> Create(BomDetailDto model);
        Task<ResponseModel<BomDetailDto?>> Modify(BomDetailDto model);
        Task<ResponseModel<BomDetailDto?>> Delete(long? BomDetailId, byte[] row_version, long createdBy);
    }
    [ScopedRegistration]
    public class BomDetailService : IBomDetailService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public BomDetailService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetByBomID(PageModel pageInfo, long? BomId, long? MaterialId, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_BomDetail_GetByBomId"; 
                var param = new DynamicParameters();
                param.Add("@BomId", BomId);
                param.Add("@MaterialId", MaterialId);
                param.Add("@showDelete", showDelete);
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<dynamic>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
                return returnData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseModel<BomDetailDto?>> Create(BomDetailDto model)
        {
            var returnData = new ResponseModel<BomDetailDto?>();

            var validator = new BomDetailValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_BomDetail_Create";
            var param = new DynamicParameters();
            param.Add("@BomDetailId", model.BomDetailId);
            param.Add("@BomId", model.BomId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@Amount", model.Amount);
            param.Add("@Remark", model.Remark);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetById(model.BomDetailId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<BomDetailDto?>> Modify(BomDetailDto model)
        {
            var returnData = new ResponseModel<BomDetailDto?>();

            var validator = new BomDetailValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_BomDetail_Modify";
            var param = new DynamicParameters();
            param.Add("@BomDetailId", model.BomDetailId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@Amount", model.Amount);
            param.Add("@Remark", model.Remark);
            param.Add("@createdBy", model.createdBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.REFRESH_REQUIRED:
                    returnData.HttpResponseCode = 500;
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetById(model.BomDetailId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<BomDetailDto?>> GetById(long? BomDetailId)
        {
            var returnData = new ResponseModel<BomDetailDto?>();
            string proc = "Usp_BomDetail_GetById";
            var param = new DynamicParameters();
            param.Add("@BomDetailId", BomDetailId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BomDetailDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<BomDetailDto?>> Delete(long? BomDetailId, byte[] row_version, long createdBy)
        {
            string proc = "Usp_BomDetail_Delete";
            var param = new DynamicParameters();
            param.Add("@BomDetailId", BomDetailId);
            param.Add("@row_version", row_version);
            param.Add("@createdBy", createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<BomDetailDto?>();
            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.REFRESH_REQUIRED:
                    returnData.HttpResponseCode = 500;
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SUCCESS:
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return returnData;
        }

    }
}
