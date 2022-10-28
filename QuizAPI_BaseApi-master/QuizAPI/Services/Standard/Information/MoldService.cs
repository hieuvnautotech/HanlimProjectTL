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
    public interface IMoldService
    {
        Task<ResponseModel<IEnumerable<MoldDto>?>> GetAll(PageModel pageInfo, string keyWord, long? Model, long? MoldType, long? MachineType, bool showDelete);
        Task<ResponseModel<MoldDto?>> GetById(long? MoldId);
        Task<ResponseModel<MoldDto?>> Create(MoldDto model);
        Task<ResponseModel<MoldDto?>> Modify(MoldDto model);
        Task<ResponseModel<MoldDto?>> Delete(long? MoldId, byte[] row_version , long user);
    }
    [ScopedRegistration]
    public class MoldService : IMoldService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public MoldService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<MoldDto>?>> GetAll(PageModel pageInfo, string keyWord, long? Model, long? MoldType, long? MachineType, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<MoldDto>?>();
                string proc = "Usp_Mold_GetAll"; var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
                param.Add("@Model", Model);
                param.Add("@MoldType", MoldType);
                param.Add("@MachineType", MachineType);
                param.Add("@showDelete", showDelete);
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MoldDto>(proc, param);
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

        public async Task<ResponseModel<MoldDto?>> Create(MoldDto model)
        {
            var returnData = new ResponseModel<MoldDto?>();

            var validator = new MoldValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Mold_Create";
            var param = new DynamicParameters();
            param.Add("@MoldId", model.MoldId);
            param.Add("@MoldSerial", model.MoldSerial);
            param.Add("@MoldCode", model.MoldCode);
            param.Add("@Model", model.Model);
            param.Add("@MoldType", model.MoldType);
            param.Add("@Inch", model.Inch);
            param.Add("@MachineType", model.MachineType);
            param.Add("@MachineTon", model.MachineTon);
            param.Add("@ETADate", model.ETADate);
            param.Add("@Cabity", model.Cabity);
            param.Add("@ETAStatus", model.ETAStatus);
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
                    returnData = await GetById(model.MoldId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<MoldDto?>> Modify(MoldDto model)
        {
            var returnData = new ResponseModel<MoldDto?>();

            var validator = new MoldValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Mold_Modify";
            var param = new DynamicParameters();
            param.Add("@MoldId", model.MoldId);
            param.Add("@MoldSerial", model.MoldSerial);
            param.Add("@MoldCode", model.MoldCode);
            param.Add("@Model", model.Model);
            param.Add("@MoldType", model.MoldType);
            param.Add("@Inch", model.Inch);
            param.Add("@MachineType", model.MachineType);
            param.Add("@MachineTon", model.MachineTon);
            param.Add("@ETADate", model.ETADate);
            param.Add("@Cabity", model.Cabity);
            param.Add("@ETAStatus", model.ETAStatus);
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
                    returnData = await GetById(model.MoldId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<MoldDto?>> GetById(long? MoldId)
        {
            var returnData = new ResponseModel<MoldDto?>();
            string proc = "Usp_Mold_GetById";
            var param = new DynamicParameters();
            param.Add("@MoldId", MoldId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MoldDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<MoldDto?>> Delete(long? MoldId, byte[] row_version, long user)
        {
            string proc = "Usp_Mold_Delete";
            var param = new DynamicParameters();
            param.Add("@MoldId", MoldId);
            param.Add("@row_version", row_version);
            param.Add("@createdBy", user);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<MoldDto?>();
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
