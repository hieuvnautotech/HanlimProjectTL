using Dapper;
using Newtonsoft.Json;
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
    public interface IBomService
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? MaterialId, bool showDelete);
        Task<ResponseModel<IEnumerable<dynamic>?>> GetForCopy(PageModel pageInfo, long BomId);
        Task<ResponseModel<BomDto?>> GetById(long? BomId);
        Task<ResponseModel<BomDto?>> Create(BomDto model);
        Task<ResponseModel<BomDto?>> Modify(BomDto model);
        Task<ResponseModel<BomDto?>> Delete(long? BomId, byte[] row_version, long user);
        Task<ResponseModel<BomDto?>> Copy(List<BomDto> model, string Version, long BomId, long createdBy);
        Task<ResponseModel<IEnumerable<BomDto>>> GetParent(long BomId);

    }
    [ScopedRegistration]
    public class BomService : IBomService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public BomService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? MaterialId, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_Bom_GetAll"; var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
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

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetForCopy(PageModel pageInfo, long BomId)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_Bom_GetForCopy"; var param = new DynamicParameters();
                param.Add("@BomId", BomId);
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

        public async Task<ResponseModel<BomDto?>> Create(BomDto model)
        {
            var returnData = new ResponseModel<BomDto?>();

            var validator = new BomValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Bom_Create";
            var param = new DynamicParameters();
            param.Add("@BomId", model.BomId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@ParentId", model.ParentId);
            param.Add("@Amount", model.Amount);
            param.Add("@Remark", model.Remark);
            param.Add("@Version", model.Version);
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
                    returnData = await GetById(model.BomId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<BomDto?>> Modify(BomDto model)
        {
            var returnData = new ResponseModel<BomDto?>();

            var validator = new BomValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Bom_Modify";
            var param = new DynamicParameters();
            param.Add("@BomId", model.BomId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@Remark", model.Remark);
            param.Add("@Amount", model.Amount);
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
                    returnData = await GetById(model.BomId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<BomDto?>> GetById(long? BomId)
        {
            var returnData = new ResponseModel<BomDto?>();
            string proc = "Usp_Bom_GetById";
            var param = new DynamicParameters();
            param.Add("@BomId", BomId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BomDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<BomDto?>> Delete(long? BomId, byte[] row_version, long user)
        {
            string proc = "Usp_Bom_Delete";
            var param = new DynamicParameters();
            param.Add("@BomId", BomId);
            param.Add("@row_version", row_version);
            param.Add("@createdBy", user);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<BomDto?>();
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

        public async Task<ResponseModel<BomDto?>> Copy(List<BomDto> model, string Version, long BomId, long createdBy)
        {
            try
            {
                var returnData = new ResponseModel<BomDto?>();
                var jsonLotList = JsonConvert.SerializeObject(model);

                string proc = "Usp_Bom_Copy";
                var param = new DynamicParameters();
                param.Add("@BomId", BomId);
                param.Add("@Version", Version);
                param.Add("@Jsonlist", jsonLotList);
                param.Add("@createdBy", createdBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.SYSTEM_ERROR:
                        returnData.HttpResponseCode = 500;
                        break;
                    case StaticReturnValue.SUCCESS:
                        //returnData = await GetById(model.BomId);
                        returnData.ResponseMessage = result;
                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }
                return returnData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseModel<IEnumerable<BomDto>>> GetParent(long BomId)
        {
            var returnData = new ResponseModel<IEnumerable<BomDto>>();
            var proc = $"Usp_Bom_GetParent";
            var param = new DynamicParameters();
            param.Add("@BomId", BomId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BomDto>(proc, param);
            returnData.Data = data;

            return returnData;
        }
    }
}
