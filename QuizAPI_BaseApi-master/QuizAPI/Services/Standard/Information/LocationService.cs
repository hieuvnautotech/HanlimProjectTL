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
    public interface ILocationService
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? AreaId, bool showDelete);
        Task<ResponseModel<LocationDto?>> GetById(long? LocationId);
        Task<ResponseModel<LocationDto?>> Create(LocationDto model);
        Task<ResponseModel<LocationDto?>> Modify(LocationDto model);
        Task<ResponseModel<LocationDto?>> Delete(LocationDto model);
        Task<ResponseModel<IEnumerable<LocationDto>?>> GetActive(LocationDto model);

    }
    [ScopedRegistration]
    public class LocationService : ILocationService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public LocationService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? AreaId, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_Location_GetAll"; var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
                param.Add("@AreaId", AreaId);
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

        public async Task<ResponseModel<LocationDto?>> Create(LocationDto model)
        {
            var returnData = new ResponseModel<LocationDto?>();

            var validator = new LocationValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Location_Create";
            var param = new DynamicParameters();
            param.Add("@LocationId", model.LocationId);
            param.Add("@LocationCode", model.LocationCode);
            param.Add("@AreaId", model.AreaId);
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
                    returnData = await GetById(model.LocationId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<LocationDto?>> GetById(long? LocationId)
        {
            var returnData = new ResponseModel<LocationDto?>();
            string proc = "Usp_Location_GetById";
            var param = new DynamicParameters();
            param.Add("@LocationId", LocationId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<LocationDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<LocationDto?>> Modify(LocationDto model)
        {
            var returnData = new ResponseModel<LocationDto?>();

            var validator = new LocationValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Location_Modify";
            var param = new DynamicParameters();
            param.Add("@LocationId", model.LocationId);
            param.Add("@LocationCode", model.LocationCode);
            param.Add("@AreaId", model.AreaId);
            param.Add("@modifiedBy", model.modifiedBy);
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
                    returnData = await GetById(model.LocationId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }
        public async Task<ResponseModel<LocationDto?>> Delete(LocationDto model)
        {
            string proc = "Usp_Location_Delete";
            var param = new DynamicParameters();
            param.Add("@LocationId", model.LocationId);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@isActived", model.isActived);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<LocationDto?>();
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

        public async Task<ResponseModel<IEnumerable<LocationDto>?>> GetActive(LocationDto model)
        {
            var returnData = new ResponseModel<IEnumerable<LocationDto>?>();
            var proc = $"Usp_Location_GetActive";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<LocationDto>(proc);

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
