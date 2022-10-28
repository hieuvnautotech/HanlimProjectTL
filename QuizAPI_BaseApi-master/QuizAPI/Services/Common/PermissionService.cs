using Dapper;
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
    public interface IPermissionService
    {
        Task<ResponseModel<IEnumerable<PermissionDto>?>> GetAll(PageModel pageInfo, long userId, string keyWord);
        Task<ResponseModel<IEnumerable<PermissionDto>?>> GetByRole(PageModel pageInfo, long roleId, string keyWord);
        Task<ResponseModel<PermissionDto?>> GetById(long permissionId);
        Task<string> Create(PermissionDto model);
        Task<string> Modify(PermissionDto model);
        Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect();
    }

    [ScopedRegistration]
    public class PermissionService : IPermissionService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IUserService _userService;

        public PermissionService(ISqlDataAccess sqlDataAccess, IUserService userService)
        {
            _sqlDataAccess = sqlDataAccess;
            _userService = userService;
        }

        public async Task<string> Create(PermissionDto model)
        {
            string proc = "sysUsp_Permission_Create";
            var param = new DynamicParameters();
            param.Add("@commonDetailId", model.commonDetailId);
            param.Add("@forRoot", model.forRoot);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<PermissionDto>?>> GetAll(PageModel pageInfo, long userId, string keyWord)
        {
            var userRole = await _userService.GetUserRole(userId);
            string proc;
            var returnData = new ResponseModel<IEnumerable<PermissionDto>?>();
            if (userRole.Contains(RoleConst.ROOT))
            {
             
                proc = "sysUsp_Permission_GetAll";
                var param = new DynamicParameters();
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@keyWord", keyWord);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PermissionDto>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
              
            }
            else
            {
                proc = "sysUsp_Permission_GetExceptRoot";
            }
            return returnData;
            
        }

        public async Task<ResponseModel<IEnumerable<PermissionDto>?>> GetByRole(PageModel pageInfo, long roleId, string keyWord)
        {
            var returnData = new ResponseModel<IEnumerable<PermissionDto>?>();
            string proc = "sysUsp_Permission_GetByRole";
            var param = new DynamicParameters();
            param.Add("@roleId", roleId);
            param.Add("@page", pageInfo.page);
            param.Add("@keyword", keyWord);
            param.Add("@pageSize", pageInfo.pageSize);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PermissionDto>(proc, param);
            returnData.Data = data;
            returnData.TotalRow = param.Get<int>("totalRow");
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<PermissionDto?>> GetById(long permissionId)
        {
            var returnData = new ResponseModel<PermissionDto?>();
            string proc = "sysUsp_Permission_GetById";
            var param = new DynamicParameters();
            param.Add("@permissionId", permissionId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PermissionDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> Modify(PermissionDto model)
        {
            try
            {
                if (!ValidateModel.CheckValid(model, new sysTbl_Permission()))
                {
                    return StaticReturnValue.OBJECT_INVALID;
                }

                string proc = "sysUsp_Permission_Modify";
                var param = new DynamicParameters();
                param.Add("@permissionId", model.permissionId);
                param.Add("@forRoot", model.forRoot);
                param.Add("@modifiedBy", model.modifiedBy);
                param.Add("@row_version", model.row_version);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);
                //luôn để DataOutput trong stored procedure

                return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
            }
            catch (Exception e)
            {

                throw;
            }
           
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect()
        {
            var returnData = new ResponseModel<IEnumerable<dynamic>?>();
            var proc = $"sysUsp_Permission_GetForSelect";

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<dynamic>(proc);
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
