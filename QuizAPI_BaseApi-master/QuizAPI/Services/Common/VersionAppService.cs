using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using System.Data;
using System.Numerics;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface IVersionAppService
    {
        Task<ResponseModel<IEnumerable<VersionAppDto>?>> CheckVersionApp(int versionCode);
        Task<ResponseModel<VersionAppDto?>> GetAll();
        Task<string> Modify(VersionAppDto model);
    }

    [ScopedRegistration]
    public class VersionAppService : IVersionAppService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public VersionAppService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<VersionAppDto>?>> CheckVersionApp(int versionCode)
        {
            var returnData = new ResponseModel<IEnumerable<VersionAppDto>?>();
            string proc = "sysUsp_VersionApp_GetAll";
            var param = new DynamicParameters();
            param.Add("@versionCode", versionCode);
       
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<VersionAppDto>(proc, param);
            returnData.Data = data;
            returnData.ResponseMessage = param.Get<string>("output");
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }
        public async Task<ResponseModel<VersionAppDto?>> GetAll()
        {

            var returnData = new ResponseModel<VersionAppDto?>();
            var proc = $"sysUsp_VersionApp_GetActive";
            var param = new DynamicParameters();
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<VersionAppDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> Modify(VersionAppDto model)
        {
            string proc = "sysUsp_VersionApp_Modify";
            var param = new DynamicParameters();
            param.Add("@id_app", model.id_app);
            param.Add("@version", model.version);
            param.Add("@name_file", model.name_file);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
    }
}
