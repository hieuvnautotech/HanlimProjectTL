using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface ILoginService
    {
        Task<string> CheckLogin(LoginModelDto model);
       
    }

    [ScopedRegistration]
    public class LoginService : ILoginService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public LoginService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<string> CheckLogin(LoginModelDto model)
        {

            model.userPassword = MD5Encryptor.MD5Hash(model.userPassword);

            string proc = "sysUsp_User_CheckLogin";
            var param = new DynamicParameters();
            param.Add("@userName", model.userName);
            param.Add("@userPassword", model.userPassword);
            //param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output);//luôn để DataOutput trong stored procedure

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<string>(proc, param);
            return data.FirstOrDefault() ?? string.Empty;
        }
    }
}
