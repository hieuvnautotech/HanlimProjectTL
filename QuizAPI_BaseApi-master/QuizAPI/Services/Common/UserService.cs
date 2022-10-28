using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models;
using QuizAPI.Models.Dtos.Common;
using System.Data;
using System.Numerics;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface IUserService
    {
        Task<string> Create(UserDto model);
        Task<ResponseModel<UserDto?>> GetByUserId(long userInfoId = 0);
        Task<ResponseModel<UserDto?>> GetByUserName(string? userName = null);
        Task<string> ChangeUserPassword(UserDto model);
        Task<string> ChangeUserPasswordByRoot(UserDto model);
        Task<string> SetLastLoginOnWeb(UserDto model);
        Task<string> SetLastLoginOnApp(UserDto model);
        Task<string> SetUserInfoRole(UserDto model);
        Task<IEnumerable<string>> GetUserRole(long userId);
        Task<IEnumerable<dynamic>> GetRoleByUser(long userId);
        Task<IEnumerable<string>> GetUserPermission(long userId);
        Task<IEnumerable<MenuDto>?> GetUserMenu(long userId);
        Task<UserDto?> GetUserInfo(long userId = 0);
        Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyword);
        Task<ResponseModel<IEnumerable<UserDto>?>> GetExceptRoot(PageModel pageInfo, string keyword);
        Task<string> Delete(long userId);
    }

    [ScopedRegistration]
    public class UserService : IUserService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public UserService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<string> ChangeUserPassword(UserDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_User()) || string.IsNullOrWhiteSpace(model.newPassword))
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            model.userPassword = MD5Encryptor.MD5Hash(model.userPassword);
            model.newPassword = MD5Encryptor.MD5Hash(model.newPassword);

            string proc = "sysUsp_User_ChangePassword";
            var param = new DynamicParameters();
            param.Add("@userName", model.userName);
            param.Add("@userPassword", model.userPassword);
            param.Add("@newPassword", model.newPassword);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> ChangeUserPasswordByRoot(UserDto model)
        {
            if (string.IsNullOrWhiteSpace(model.newPassword))
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            model.newPassword = MD5Encryptor.MD5Hash(model.newPassword);

            string proc = "sysUsp_User_ChangePasswordByRoot";
            var param = new DynamicParameters();
            param.Add("@UserId", model.userId);
            param.Add("@newPassword", model.newPassword);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> Create(UserDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_User()))
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            model.userPassword = MD5Encryptor.MD5Hash(model.userPassword);

            // hash password
            //model.userPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(model.userPassword);

            string proc = "sysUsp_User_Create";
            var param = new DynamicParameters();
            param.Add("@userId", model.userId);
            param.Add("@userName", model.userName);
            param.Add("@userPassword", model.userPassword);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> Update(UserDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_User()))
            {
                return StaticReturnValue.OBJECT_INVALID;
            }

            model.userPassword = MD5Encryptor.MD5Hash(model.userPassword);

            // hash password
            //model.userPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(model.userPassword);

            string proc = "sysUsp_User_Create";
            var param = new DynamicParameters();
            param.Add("@userId", model.userId);
            param.Add("@userName", model.userName);
            param.Add("@userPassword", model.userPassword);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<ResponseModel<UserDto?>> GetByUserId(long userId = 0)
        {
            var returnData = new ResponseModel<UserDto?>();
            string proc = "sysUsp_User_GetById";
            var param = new DynamicParameters();
            param.Add("@userId", userId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<UserDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<UserDto?>> GetByUserName(string? userName = null)
        {
            var returnData = new ResponseModel<UserDto?>();
            string proc = "sysUsp_User_GetByUserName";
            var param = new DynamicParameters();
            param.Add("@userName", userName);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<UserDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> SetUserInfoRole(UserDto model)
        {
            try
            {
                if (!model.Roles.Any())
                {
                    return StaticReturnValue.OBJECT_INVALID;
                }

                var roleIds = new List<long>();
                foreach (var role in model.Roles)
                {
                    roleIds.Add(role.roleId);
                }

                string proc = "sysUsp_User_SetRoles";
                var param = new DynamicParameters();
                param.Add("@userId", model.userId);
                param.Add("@roleIds", ParameterTvp.GetTableValuedParameter_BigInt(roleIds));
                param.Add("@modifiedBy", model.modifiedBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

                var a = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
                return a;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetUserPermission(long userId)
        {
            string proc = "sysUsp_User_GetPermissions";
            var param = new DynamicParameters();
            param.Add("@userId", userId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<string>(proc, param);
            return data;
        }

        public async Task<IEnumerable<string>> GetUserRole(long userId)
        {
            string proc = "sysUsp_User_GetRoles";
            var param = new DynamicParameters();
            param.Add("@userId", userId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<string>(proc, param);
            return data;
        }

        public async Task<IEnumerable<dynamic>> GetRoleByUser(long userId)
        {
            string proc = "sysUsp_User_GetRoles";
            var param = new DynamicParameters();
            param.Add("@userId", userId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<dynamic>(proc, param);
            return data;
        }

        public async Task<UserDto?> GetUserInfo(long userId = 0)
        {
            var tempData = await GetByUserId(userId);
            if (tempData.Data == null)
            {
                return null;
            }
            else
            {
                var user = tempData.Data;
                //user.RoleNames = await GetUserRole(user.userId);
                //user.PermissionNames = await GetUserPermission(user.userId);
                user.Menus = await GetUserMenu(user.userId);
                return user; 
            }
        }

        public async Task<string> SetLastLoginOnWeb(UserDto model)
        {
            string proc = "sysUsp_User_SetLastLoginOnWeb";
            var param = new DynamicParameters();
            param.Add("@UserId", model.userId);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure
            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo , string keyword)
        {
            var returnData = new ResponseModel<IEnumerable<dynamic>?>();
            string proc = "sysUsp_User_GetAll";
            var param = new DynamicParameters();
            param.Add("@keyword", keyword);
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

        public async Task<ResponseModel<IEnumerable<UserDto>?>> GetExceptRoot(PageModel pageInfo, string keyword)
        {
            var returnData = new ResponseModel<IEnumerable<UserDto>?>();
            string proc = "sysUsp_User_GetExceptRoot";
            var param = new DynamicParameters();
            param.Add("@keyword", keyword);
            param.Add("@page", pageInfo.page);
            param.Add("@pageSize", pageInfo.pageSize);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<UserDto>(proc, param);
            returnData.Data = data;
            returnData.TotalRow = param.Get<int>("totalRow");
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> SetLastLoginOnApp(UserDto model)
        {
            string proc = "sysUsp_User_SetLastLoginOnApp";
            var param = new DynamicParameters();
            param.Add("@userId", model.userId);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure
            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<IEnumerable<MenuDto>?> GetUserMenu(long userId)
        {
            var roleList = await GetUserRole(userId);
            if (!roleList.Any())
            {
                return null;
            }
            else
            {
                var proc = $"sysUsp_Menu_GetByUserId";
                var param = new DynamicParameters();
                param.Add("@RoleList", ParameterTvp.GetTableValuedParameter_NVarchar(roleList));

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MenuDto>(proc, param);
                return data;
            }

            //throw new NotImplementedException();
        }

        public async Task<string> Delete(long userId)
        {
            string proc = "sysUsp_User_Delete";
            var param = new DynamicParameters();
            param.Add("@userId", userId);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
    }
}
