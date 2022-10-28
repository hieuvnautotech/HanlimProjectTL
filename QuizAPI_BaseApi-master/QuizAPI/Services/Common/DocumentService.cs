using Dapper;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Base;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface IDocumentService
    {
        Task<ResponseModel<IEnumerable<DocumentDto>?>> GetAll(PageModel pageInfo, string keyWord, string language, bool showDelete = true);
        Task<ResponseModel<DocumentDto?>> GetById(long? DocumentId);
        Task<ResponseModel<DocumentDto>> GetByComponent(string menuComponent,string language);
        Task<ResponseModel<DocumentDto?>> Create(DocumentDto model);
        Task<ResponseModel<DocumentDto?>> Modify(DocumentDto model);
        Task<string> Delete(DocumentDto model);
    }

    [ScopedRegistration]
    public class DocumentService : IDocumentService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public DocumentService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<DocumentDto>?>> GetAll(PageModel pageInfo, string keyWord, string language, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<DocumentDto>?>();
                string proc = "sysUsp_Document_GetAll";
                var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
                param.Add("@language", language);
                param.Add("@showDelete", showDelete);
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<DocumentDto>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
                return returnData;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<ResponseModel<DocumentDto?>> Create(DocumentDto model)
        {
            try
            {
                var returnData = new ResponseModel<DocumentDto?>();
                string proc = "sysUsp_Document_Create";
                var param = new DynamicParameters();
                param.Add("@documentId", model.documentId);
                param.Add("@menuComponent", model.menuComponent);
                param.Add("@urlFile", model.urlFile);
                param.Add("@language", model.language);
                param.Add("@createdBy", model.createdBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

                var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);

                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.SYSTEM_ERROR:
                        returnData.HttpResponseCode = 500;
                        break;
                    case StaticReturnValue.SUCCESS:
                        returnData = await GetById(model.documentId);
                        returnData.ResponseMessage = result;
                        break;
                    default:
                        returnData.HttpResponseCode = 400;
                        break;
                }
                return returnData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> Delete(DocumentDto model)
        {
            string proc = "sysUsp_Document_Delete";
            var param = new DynamicParameters();
            param.Add("@documentId", model.documentId);
            param.Add("@createdBy", model.createdBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<DocumentDto?>> GetById(long? DocumentId)
        {
            var returnData = new ResponseModel<DocumentDto?>();
            string proc = "sysUsp_Document_GetById";
            var param = new DynamicParameters();
            param.Add("@documentId", DocumentId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<DocumentDto?>(proc, param);
            returnData.Data = data.FirstOrDefault();
            returnData.ResponseMessage = StaticReturnValue.SUCCESS;
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<DocumentDto>> GetByComponent(string menuComponent, string language)
        {
            var returnData = new ResponseModel<DocumentDto>();

            string proc = "sysUsp_Document_GetByComponent";
            var param = new DynamicParameters();
            param.Add("@menuComponent", menuComponent);
            param.Add("@language", language);

            returnData.Data = (await _sqlDataAccess.LoadDataUsingStoredProcedure<DocumentDto>(proc, param)).FirstOrDefault();
            return returnData;
        }

        public async Task<ResponseModel<DocumentDto?>> Modify(DocumentDto model)
        {
            var returnData = new ResponseModel<DocumentDto?>();

            string proc = "sysUsp_Document_Modify";
            var param = new DynamicParameters();
            param.Add("@documentId", model.documentId);
            param.Add("@menuComponent", model.menuComponent);
            param.Add("@urlFile", model.urlFile);
            param.Add("@language", model.language);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetById(model.documentId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }
    }
}
