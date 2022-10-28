using Dapper;
using Microsoft.AspNetCore.Mvc;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Base;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common.Standard.Information
{
    public interface IStandardQCService
    {
        Task<ResponseModel<IEnumerable<StandardQCDto>?>> GetAll(StandardQCDto pageInfo);
        Task<ResponseModel<IEnumerable<StandardQCDto>?>> GetActive(StandardQCDto model);
        Task<ResponseModel<StandardQCDto?>> GetById(long QCId);
        Task<string> Create(StandardQCDto model);
        Task<string> Modify(StandardQCDto model);
        Task<string> Delete(StandardQCDto model);

    }
    [ScopedRegistration]
    public class StandardQCService : IStandardQCService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public StandardQCService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<ResponseModel<IEnumerable<StandardQCDto>?>> GetAll(StandardQCDto model)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<StandardQCDto>?>();
                string proc = "Usp_StandardQC_GetAll"; var param = new DynamicParameters();
                param.Add("@page", model.page);
                param.Add("@pageSize", model.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@QCCode", model.QCCode);
                param.Add("@Description", model.Description);
                param.Add("@showDelete", model.showDelete);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StandardQCDto>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
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
        public async Task<ResponseModel<StandardQCDto?>> GetById(long QCId)
        {
            var returnData = new ResponseModel<StandardQCDto?>();
            var proc = $"Usp_StandardQC_GetById";
            var param = new DynamicParameters();
            param.Add("@QCId", QCId);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StandardQCDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }
        public async Task<string> Create(StandardQCDto model)
        {
            string proc = "Usp_StandardQC_Create";
            var param = new DynamicParameters();
            param.Add("@QCId", model.QCId);
            param.Add("@QCCode", model.QCCode);
            param.Add("@Description", model.Description);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<string> Modify(StandardQCDto model)
        {
            string proc = "Usp_StandardQC_Modify";
            var param = new DynamicParameters();
            param.Add("@QCId", model.QCId);
            param.Add("@QCCode", model.QCCode);
            param.Add("@Description", model.Description);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<string> Delete(StandardQCDto model)
        {
            string proc = "Usp_StandardQC_Delete";
            var param = new DynamicParameters();
            param.Add("@QCId", model.QCId);
            param.Add("@row_version", model.row_version);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<StandardQCDto>?>> GetActive(StandardQCDto model)
        {
            var returnData = new ResponseModel<IEnumerable<StandardQCDto>?>();
            var proc = $"Usp_StandardQC_GetActive";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<StandardQCDto>(proc);

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
