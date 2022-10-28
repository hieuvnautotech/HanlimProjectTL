using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Validators;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Standard.Information
{
    public interface ITrayService
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? TrayType, bool showDelete);
        Task<ResponseModel<TrayDto?>> GetById(long? TrayId);
        Task<ResponseModel<TrayDto?>> Create(TrayDto model);
        Task<ResponseModel<TrayDto?>> Modify(TrayDto model);
        Task<ResponseModel<TrayDto?>> Delete(long? TrayId, byte[] row_version, long user);
    }
    [ScopedRegistration]
    public class TrayService : ITrayService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public TrayService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? TrayType, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_Tray_GetAll"; var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
                param.Add("@TrayType", TrayType);
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

        public async Task<ResponseModel<TrayDto?>> Create(TrayDto model)
        {
            var returnData = new ResponseModel<TrayDto?>();

            var validator = new TrayValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Tray_Create";
            var param = new DynamicParameters();
            param.Add("@TrayId", model.TrayId);
            param.Add("@TrayCode", model.TrayCode);
            param.Add("@TrayType", model.TrayType);
            param.Add("@IsReuse", model.IsReuse);
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
                    returnData = await GetById(model.TrayId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<TrayDto?>> Modify(TrayDto model)
        {
            var returnData = new ResponseModel<TrayDto?>();

            var validator = new TrayValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Tray_Modify";
            var param = new DynamicParameters();
            param.Add("@TrayId", model.TrayId);
            param.Add("@TrayCode", model.TrayCode);
            param.Add("@TrayType", model.TrayType);
            param.Add("@IsReuse", model.IsReuse);
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
                    returnData = await GetById(model.TrayId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<TrayDto?>> GetById(long? TrayId)
        {
            var returnData = new ResponseModel<TrayDto?>();
            string proc = "Usp_Tray_GetById";
            var param = new DynamicParameters();
            param.Add("@TrayId", TrayId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<TrayDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<TrayDto?>> Delete(long? TrayId, byte[] row_version, long user)
        {
            string proc = "Usp_Tray_Delete";
            var param = new DynamicParameters();
            param.Add("@TrayId", TrayId);
            param.Add("@row_version", row_version);
            param.Add("@createdBy", user);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<TrayDto?>();
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
