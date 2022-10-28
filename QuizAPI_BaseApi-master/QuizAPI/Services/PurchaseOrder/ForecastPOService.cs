using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Dtos;
using static QuizAPI.Extensions.ServiceExtensions;
using QuizAPI.DbAccess;
using Dapper;
using System.Data;
using QuizAPI.Extensions;
using QuizAPI.Models.Validators;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuizAPI.Services.PurchaseOrder
{
    public interface IForecastPOService
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, int keyWordWeekStart, int keyWordWeekEnd, int keyWordYear, bool showDelete);
        Task<ResponseModel<ForecastPODto?>> GetById(long? FPOId);
        Task<string> Create(ForecastPODto model);
        Task<string> Modify(ForecastPODto model);
        Task<ResponseModel<IEnumerable<SelectMaterial>?>> SelectBoxMaterial();
        Task<ResponseModel<IEnumerable<SelectLine>?>> SelectBoxLine();
        Task<ResponseModel<IEnumerable<SelectYear>?>> SelectBoxYear();
        Task<ResponseModel<ForecastPODto?>> Delete(long? FPOId, byte[] row_version, long user);
    }
    [ScopedRegistration]

    public class ForecastPOService : IForecastPOService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public ForecastPOService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, int keyWordWeekStart, int keyWordWeekEnd, int keyWordYear,  bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_ForecastPO_GetAll"; var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
                param.Add("@keywordweekstart", keyWordWeekStart);
                param.Add("@keywordweekend", keyWordWeekEnd);
                param.Add("@keywordyear", keyWordYear);
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
        
        public async Task<string> Create(ForecastPODto model)
        {
            string proc = "Usp_ForecastPO_Create";
            var param = new DynamicParameters();
            param.Add("@FPOId", model.FPOId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@LineId", model.LineId);
            param.Add("@Week", model.Week);
            param.Add("@Year", model.Year);
            param.Add("@Amount", model.Amount);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
            //throw new NotImplementedException();
        }
        public async Task<ResponseModel<ForecastPODto?>> GetById(long? FPOId)
        {
            var returnData = new ResponseModel<ForecastPODto?>();
            var proc = $"Usp_ForecastPO_GetById";
            var param = new DynamicParameters();
            param.Add("@FPOId", FPOId);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<ForecastPODto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<IEnumerable<SelectMaterial>?>> SelectBoxMaterial()
        {
            var returnData = new ResponseModel<IEnumerable<SelectMaterial>?>();
            var proc = $"Usp_ForecastPO_GetSelectMaterial";
            //var param = new DynamicParameters();
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<SelectMaterial>(proc);
            returnData.Data = data;
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<IEnumerable<SelectLine>?>> SelectBoxLine()
        {
            var returnData = new ResponseModel<IEnumerable<SelectLine>?>();
            var proc = $"Usp_ForecastPO_GetSelectLine";
            //var param = new DynamicParameters();
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<SelectLine>(proc);
            returnData.Data = data;
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }
        public async Task<ResponseModel<IEnumerable<SelectYear>?>> SelectBoxYear()
        {
            var returnData = new ResponseModel<IEnumerable<SelectYear>?>();
            var proc = $"Usp_ForecastPO_GetSelectYear";
            //var param = new DynamicParameters();
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<SelectYear>(proc);
            returnData.Data = data;
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }
        public async Task<string> Modify(ForecastPODto model)
        {
            string proc = "Usp_ForecastPO_Modify";
            var param = new DynamicParameters();
            param.Add("@FPOId", model.FPOId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@LineId", model.LineId);
            param.Add("@Week", model.Week);
            param.Add("@Year", model.Year);
            param.Add("@Amount", model.Amount);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<ForecastPODto?>> Delete(long? FPOId, byte[] row_version, long user)
        {
            string proc = "Usp_ForecastPO_Delete";
            var param = new DynamicParameters();
            param.Add("@FPOId", FPOId);
            param.Add("@row_version", row_version);
            param.Add("@createdBy", user);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<ForecastPODto?>();
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