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

namespace QuizAPI.Services
{
    public interface ICustomService
    {
        Task<ResponseModel<IEnumerable<T>>> GetForSelect<T>(string Column, string Table, string Where, string Order);
    }
    [ScopedRegistration]
    public class CustomService : ICustomService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CustomService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<T>>> GetForSelect<T>(string Column, string Table, string Where, string Order)
        {
            var returnData = new ResponseModel<IEnumerable<T>>();
            var proc = $"Usp_All_GetForSelect";
            var param = new DynamicParameters();
            param.Add("@Column", Column);
            param.Add("@Table", Table);
            param.Add("@Where", Where);
            param.Add("@Order", Order);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<T>(proc, param);
            
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
