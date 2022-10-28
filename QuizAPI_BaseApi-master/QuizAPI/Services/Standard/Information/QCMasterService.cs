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
    public interface IQCMasterService
    {
        Task<ResponseModel<IEnumerable<QCMasterDto>?>> GetAll(QCMasterDto pageInfo);
        Task<ResponseModel<IEnumerable<QCMasterDto>?>> GetActive(QCMasterDto model);
        Task<ResponseModel<QCMasterDto?>> GetById(long QCMasterId);
        Task<string> Create(QCMasterDto model);
        Task<string> Modify(QCMasterDto model);
        Task<string> Delete(QCMasterDto mode);
        Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect(string commonMasterName);
        Task<ResponseModel<IEnumerable<dynamic>?>> GetMaterialByQCType(string qcType);

    }
    [ScopedRegistration]
    public class QCMasterService : IQCMasterService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public QCMasterService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }
        public async Task<ResponseModel<IEnumerable<QCMasterDto>?>> GetAll(QCMasterDto model)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<QCMasterDto>?>();
                string proc = "Usp_QCMaster_GetAll"; var param = new DynamicParameters();
                param.Add("@page", model.page);
                param.Add("@pageSize", model.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@QCMasterCode", model.QCMasterCode);
                param.Add("@Description", model.Description);
                param.Add("@MaterialId", model.MaterialId);
                param.Add("@QCType", model.QCType);
                param.Add("@showDelete", model.showDelete);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<QCMasterDto>(proc, param);
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
        public async Task<ResponseModel<QCMasterDto?>> GetById(long QCMasterId)
        {
            var returnData = new ResponseModel<QCMasterDto?>();
            var proc = $"Usp_QCMaster_GetById";
            var param = new DynamicParameters();
            param.Add("@QCMasterId", QCMasterId);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<QCMasterDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }
        public async Task<string> Create(QCMasterDto model)
        {
            string proc = "Usp_QCMaster_Create";
            var param = new DynamicParameters();
            param.Add("@QCMasterId", model.QCMasterId);
            param.Add("@QCMasterCode", model.QCMasterCode);
            param.Add("@Description", model.Description);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@QCType", model.QCType);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<string> Modify(QCMasterDto model)
        {
            string proc = "Usp_QCMaster_Modify";
            var param = new DynamicParameters();
            param.Add("@QCMasterId", model.QCMasterId);
            param.Add("@QCMasterCode", model.QCMasterCode);
            param.Add("@Description", model.Description);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@QCType", model.QCType);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<string> Delete(QCMasterDto model)
        {
            string proc = "Usp_QCMaster_Delete";
            var param = new DynamicParameters();
            param.Add("@QCMasterId", model.QCMasterId);
            param.Add("@row_version", model.row_version);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<QCMasterDto>?>> GetActive(QCMasterDto model)
        {
            var returnData = new ResponseModel<IEnumerable<QCMasterDto>?>();
            var proc = $"Usp_QCMaster_GetActive";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<QCMasterDto>(proc);

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

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetMaterialByQCType(string qcType)
        {
            var returnData = new ResponseModel<IEnumerable<dynamic>?>();
            var proc = $"Usp_MaterialByQCType_GetActive";
            var param = new DynamicParameters();
            param.Add("@qcType", qcType);

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
