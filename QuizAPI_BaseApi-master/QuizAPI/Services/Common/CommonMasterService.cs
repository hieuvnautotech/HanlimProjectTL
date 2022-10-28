using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Base;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface ICommonMasterService : IBaseService<CommonMasterDto>
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect(string commonMasterName);
    }

    [ScopedRegistration]
    public class CommonMasterService : ICommonMasterService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CommonMasterService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<string> Create(CommonMasterDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_CommonMaster()))
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            string proc = "sysUsp_CommonMaster_Create";
            var param = new DynamicParameters();
            param.Add("@commonMasterId", model.commonMasterId);
            param.Add("@commonMasterName", model.commonMasterName?.ToUpper());
            param.Add("@createdBy", model.createdBy);
            param.Add("@forRoot", model.forRoot);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> Delete(long commonMasterId, byte[] row_version)
        {
            string proc = "sysUsp_CommonMaster_Delete";
            var param = new DynamicParameters();
            param.Add("@commonMasterId", commonMasterId);
            param.Add("@row_version", row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
        public async Task<ResponseModel<IEnumerable<CommonMasterDto>?>> GetAll(PageModel pageInfo, string roleName, string? keyWord, bool showDelete )
        {
            var returnData = new ResponseModel<IEnumerable<CommonMasterDto>?>();
            string proc = "sysUsp_CommonMaster_GetAll";
            var param = new DynamicParameters();
            var roleList = roleName.Split(",");
            if (roleList.Contains(RoleConst.ROOT))
            {
                param.Add("@forRoot", 1);
            }
            else
            {
                param.Add("@forRoot", 0);
            }

            param.Add("@page", pageInfo.page);
            param.Add("@pageSize", pageInfo.pageSize);
            param.Add("@keyWord", keyWord);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
            param.Add("@showDelete", showDelete);
        

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<CommonMasterDto>(proc, param);
            returnData.Data = data;
            returnData.TotalRow = param.Get<int>("totalRow");
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<CommonMasterDto?>> GetById(long commonMasterId)
        {
            var returnData = new ResponseModel<CommonMasterDto?>();
            string proc = "sysUsp_CommonMaster_GetById";
            var param = new DynamicParameters();
            param.Add("@commonMasterId", commonMasterId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<CommonMasterDto?>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> Modify(CommonMasterDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_CommonMaster()))
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            string proc = "sysUsp_CommonMaster_Modify";
            var param = new DynamicParameters();
            param.Add("@commonMasterId", model.commonMasterId);
            param.Add("@commonMasterName", model.commonMasterName?.ToUpper());
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@forRoot", model.forRoot);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }
        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect(string commonMasterName)
        {
            var returnData = new ResponseModel<IEnumerable<dynamic>?>();
            var proc = $"sysUsp_CommonMaster_GetForSelect";
            var param = new DynamicParameters();
            param.Add("@commonMasterName", commonMasterName);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<dynamic>(proc, param);

            returnData.Data = data;
            if (!data.Any())
            {
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                returnData.HttpResponseCode = 204;
            }

            return returnData;
        }
    }
}
