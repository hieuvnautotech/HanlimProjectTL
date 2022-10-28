using Dapper;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Base;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface IRoleService
    {
        Task<ResponseModel<RoleDto?>> GetById(long roleId);
        Task<ResponseModel<IEnumerable<RoleDto>?>> GetAll(PageModel pageInfo, string roleName, string keyWord);
        Task<string> Create(RoleDto model);
        Task<string> Modify(RoleDto model);
        Task<string> Delete(long id);
        Task<string> SetPermission(RoleDto model);
        Task<ResponseModel<IEnumerable<RoleDto>?>> GetForSelect(List<string> roles);
        Task<ResponseModel<string>> SetPermissionForRole(RoleDeleteDto model);
        Task<ResponseModel<string>> SetMenuForRole(RoleDeleteDto model);
        Task<ResponseModel<string>> DeletePermissionForRole(RoleDeleteDto model);
        Task<ResponseModel<string>> DeleteMenuForRole(RoleDeleteDto model);
    }

    [ScopedRegistration]
    public class RoleService : IRoleService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IUserService _userService;
        private readonly IMemoryCache _memoryCache;
        private readonly IUserAuthorizationService _userAuthorizationService;
        public RoleService(ISqlDataAccess sqlDataAccess, IUserService userService , IMemoryCache memoryCache, IUserAuthorizationService userAuthorizationService)
        {
            _sqlDataAccess = sqlDataAccess;
            _userService = userService;
            _memoryCache = memoryCache;
            _userAuthorizationService = userAuthorizationService;
        }

        public async Task<string> Create(RoleDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_Role()))
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            string proc = "sysUsp_Role_Create";
            var param = new DynamicParameters();
            param.Add("@roleId", model.roleId);
            param.Add("@roleName", model.roleName?.ToUpper());
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> Delete(long id)
        {
            string proc = "sysUsp_Role_Delete";
            var param = new DynamicParameters();
            param.Add("@roleId", id);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<RoleDto>?>> GetAll(PageModel pageInfo, string roleName, string keyWord)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<RoleDto>?>();
                var roleList = roleName.Split(",");
                string proc = "sysUsp_Role_GetAll";

                var param = new DynamicParameters();
                if (roleList.Contains(RoleConst.ROOT))
                {
                    param.Add("@roleRoot", "");
                }
                else
                {
                    param.Add("@roleRoot", "ROOT");
                }
                param.Add("@keyword", keyWord);
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<RoleDto>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
                return returnData;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<ResponseModel<RoleDto?>> GetById(long roleId)
        {
            var returnData = new ResponseModel<RoleDto?>();
            string proc = "sysUsp_Role_GetById";
            var param = new DynamicParameters();
            param.Add("@roleId", roleId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<RoleDto?>(proc, param);
            returnData.Data = data.FirstOrDefault();
            returnData.ResponseMessage = StaticReturnValue.SUCCESS;
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> Modify(RoleDto model)
        {

            if (!ValidateModel.CheckValid(model, new sysTbl_Role()))
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            string proc = "sysUsp_Role_Modify";
            var param = new DynamicParameters();
            param.Add("@roleId", model.roleId);
            param.Add("@roleName", model.roleName?.ToUpper());
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> SetPermission(RoleDto model)
        {
            if (!model.Permissions.Any())
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            var permissionIds = new List<long>();
            foreach (var permission in model.Permissions)
            {
                permissionIds.Add(permission.permissionId);
            }

            string proc = "sysUsp_Role_SetPermission";
            var param = new DynamicParameters();
            param.Add("@roleId", model.roleId);
            param.Add("@permissionIds", Helpers.ParameterTvp.GetTableValuedParameter_BigInt(permissionIds));
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<RoleDto>?>> GetForSelect(List<string> roles)
        {
            var returnData = new ResponseModel<IEnumerable<RoleDto>?>();
            var proc = $"sysUsp_Role_GetForSelect";
            var param = new DynamicParameters();
            if (roles.Contains(RoleConst.ROOT))
                param.Add("@isRoot", true);
            else
                param.Add("@isRoot", false);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<RoleDto>(proc, param);
            returnData.Data = data;
            if (!data.Any())
            {
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                returnData.HttpResponseCode = 204;
            }

            return returnData;
        }

        public async Task<ResponseModel<string>> SetPermissionForRole(RoleDeleteDto model)
        {
            try
            {
                var returnData = new ResponseModel<string>();
                if (!model.Permissions.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.OBJECT_INVALID;
                    return returnData;
                }

                var PermissionIds = new List<long>();
                foreach (var role in model.Permissions)
                {
                    PermissionIds.Add(role.permissionId);
                }

                string proc = "sysUsp_Role_AddPermission";
                var param = new DynamicParameters();
                param.Add("@roleId", model.roleId);
                param.Add("@permissionIds", ParameterTvp.GetTableValuedParameter_BigInt(PermissionIds));
                param.Add("@createdBy", model.createdBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                var data = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
                returnData.ResponseMessage = data;

                var userAuthorization = _userAuthorizationService.GetUserAuthorization();
                _memoryCache.Remove("userAuthorization");
                _memoryCache.Set("userAuthorization", userAuthorization.Result);

                return returnData;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseModel<string>> SetMenuForRole(RoleDeleteDto model)
        {
            try
            {
                var returnData = new ResponseModel<string>();
                if (!model.Menus.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.OBJECT_INVALID;
                    return returnData;
                }

                var MenuIds = new List<long>();
                foreach (var role in model.Menus)
                {
                    MenuIds.Add(role.menuId);
                }

                string proc = "sysUsp_Role_AddMenu";
                var param = new DynamicParameters();
                param.Add("@roleId", model.roleId);
                param.Add("@menuIds", ParameterTvp.GetTableValuedParameter_BigInt(MenuIds));
                param.Add("@createdBy", model.createdBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                var data = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
                returnData.ResponseMessage = data;
                return returnData;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseModel<string>> DeletePermissionForRole(RoleDeleteDto model)
        {
            try
            {
                var returnData = new ResponseModel<string>();
                if (!model.permissionIds.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.OBJECT_INVALID;
                    return returnData;
                }

                string proc = "sysUsp_Role_DeletePermission";
                var param = new DynamicParameters();
                param.Add("@roleId", model.roleId);
                param.Add("@permissionIds", ParameterTvp.GetTableValuedParameter_BigInt(model.permissionIds));
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                var data = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
                returnData.ResponseMessage = data;

                var userAuthorization = _userAuthorizationService.GetUserAuthorization();
                _memoryCache.Remove("userAuthorization");
                _memoryCache.Set("userAuthorization", userAuthorization.Result);

                return returnData;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseModel<string>> DeleteMenuForRole(RoleDeleteDto model)
        {
            try
            {
                var returnData = new ResponseModel<string>();
                if (!model.menuIds.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.OBJECT_INVALID;
                    return returnData;
                }

                string proc = "sysUsp_Role_DeleteMenu";
                var param = new DynamicParameters();
                param.Add("@roleId", model.roleId);
                param.Add("@menuIds", ParameterTvp.GetTableValuedParameter_BigInt(model.menuIds));
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                var data = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
                returnData.ResponseMessage = data;
                return returnData;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
