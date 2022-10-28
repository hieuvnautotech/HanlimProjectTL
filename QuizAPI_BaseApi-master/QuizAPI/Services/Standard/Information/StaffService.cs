using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;


namespace QuizAPI.Services.Standard.Information
{
    public interface IStaffService
    {
        Task<ResponseModel<IEnumerable<StaffDto>?>> Get(StaffDto model);
        Task<ResponseModel<IEnumerable<StaffDto>?>> GetAll(StaffDto model);
        Task<string> Create(StaffDto model);
        Task<ResponseModel<StaffDto?>> GetById(long staffid);
        Task<string> Modify(StaffDto model);
        Task<string> Delete(StaffDto model);
        Task<ResponseModel<IEnumerable<StaffDto>?>> GetActive(StaffDto model);
    }

    [ScopedRegistration]
    public class StaffService : IStaffService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public StaffService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<ResponseModel<IEnumerable<StaffDto>?>> Get(StaffDto model)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<StaffDto>?>();
                var proc = $"Usp_Staff_Get";
                var param = new DynamicParameters();
                param.Add("@page", model.page);
                param.Add("@pageSize", model.pageSize);
                param.Add("@StaffCode", model.StaffCode);
                param.Add("@StaffName", model.StaffName);
                param.Add("@isActived", model.isActived);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StaffDto>(proc, param);

                if (!data.Any())
                {
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                    returnData.HttpResponseCode = 204;
                }
                else
                {
                    returnData.Data = data;
                    returnData.TotalRow = param.Get<int>("totalRow");
                }

                return returnData;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseModel<IEnumerable<StaffDto>?>> GetAll(StaffDto model)
        {
            var returnData = new ResponseModel<IEnumerable<StaffDto>?>();
            var proc = $"Usp_Staff_GetAll";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StaffDto>(proc);

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
        public async Task<string> Create(StaffDto model)
        {
            string proc = "Usp_Staff_Create";
            var param = new DynamicParameters();
            param.Add("@StaffId", model.StaffId);
            param.Add("@StaffCode", model.StaffCode);
            param.Add("@StaffName", model.StaffName);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
        public async Task<ResponseModel<StaffDto?>> GetById(long staffid)
        {
            try
            {
                var returnData = new ResponseModel<StaffDto?>();
                var proc = $"Usp_Staff_GetById";
                var param = new DynamicParameters();
                param.Add("@StaffId", staffid);
                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StaffDto>(proc, param);
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
        public async Task<string> Modify(StaffDto model)
        {
            string proc = "Usp_Staff_Modify";
            var param = new DynamicParameters();
            param.Add("@StaffId", model.StaffId);
            param.Add("@StaffCode", model.StaffCode);
            param.Add("@StaffName", model.StaffName);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
        public async Task<string> Delete(StaffDto model)
        {
            string proc = "Usp_Staff_Delete";
            var param = new DynamicParameters();
            param.Add("@StaffId", model.StaffId);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@isActived", model.isActived);
            param.Add("@row_version", model.row_version);

            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
        public async Task<ResponseModel<IEnumerable<StaffDto>?>> GetActive(StaffDto model)
        {
            var returnData = new ResponseModel<IEnumerable<StaffDto>?>();
            var proc = $"Usp_Staff_GetActive";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StaffDto>(proc);

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
