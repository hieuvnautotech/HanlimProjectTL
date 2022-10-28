using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;


namespace QuizAPI.Services.Standard.Information
{
    public interface IBuyerService
    {
        Task<ResponseModel<IEnumerable<BuyerDto>?>> Get(BuyerDto model);
        Task<ResponseModel<IEnumerable<BuyerDto>?>> GetAll(BuyerDto model);
        Task<string> Create(BuyerDto model);
        Task<ResponseModel<BuyerDto?>> GetById(long buyerid);
        Task<string> Modify(BuyerDto model);
        Task<string> Delete(BuyerDto model);
        Task<ResponseModel<IEnumerable<BuyerDto>?>> GetActive(BuyerDto model);
    }

    [ScopedRegistration]
    public class BuyerService : IBuyerService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public BuyerService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<BuyerDto>?>> Get(BuyerDto model)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<BuyerDto>?>();
                var proc = $"Usp_Buyer_Get";
                var param = new DynamicParameters();
                param.Add("@page", model.page);
                param.Add("@pageSize", model.pageSize);
                param.Add("@BuyerCode", model.BuyerCode);
                param.Add("@BuyerName", model.BuyerName);
                param.Add("@isActived", model.isActived);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BuyerDto>(proc, param);

                if (!data.Any())
                {
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                    returnData.HttpResponseCode = 204;
                }
                else
                {
                    returnData.Data = data;
                    returnData.TotalRow = param.Get<int>("totalRow");
                }

                return returnData;
            }
            catch (Exception E)
            { 

                throw;
            }
            
        }
        public async Task<ResponseModel<IEnumerable<BuyerDto>?>> GetAll(BuyerDto model)
        {
            var returnData = new ResponseModel<IEnumerable<BuyerDto>?>();
            var proc = $"Usp_Buyer_GetAll";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BuyerDto>(proc);

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

        public async Task<string> Create(BuyerDto model)
        {
            string proc = "Usp_Buyer_Create";
            var param = new DynamicParameters();
            param.Add("@BuyerId", model.BuyerId);
            param.Add("@BuyerCode", model.BuyerCode);
            param.Add("@BuyerName", model.BuyerName);
            param.Add("@Contact", model.Contact);
            param.Add("@Description", model.Description);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
            //throw new NotImplementedException();
        }

        public async Task<ResponseModel<BuyerDto?>> GetById(long buyerid)
        {
            var returnData = new ResponseModel<BuyerDto?>();
            var proc = $"Usp_Buyer_GetById";
            var param = new DynamicParameters();
            param.Add("@BuyerId", buyerid);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BuyerDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> Modify(BuyerDto model)
        {
            string proc = "Usp_Buyer_Modify";
            var param = new DynamicParameters();
            param.Add("@BuyerId", model.BuyerId);
            param.Add("@BuyerCode", model.BuyerCode);
            param.Add("@BuyerName", model.BuyerName);
            param.Add("@Contact", model.Contact);
            param.Add("@Description", model.Description);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }
        public async Task<string> Delete(BuyerDto model)
        {
            string proc = "Usp_Buyer_Delete";
            var param = new DynamicParameters();
            param.Add("@BuyerId", model.BuyerId);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@isActived", model.isActived);
            param.Add("@row_version", model.row_version);

            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<BuyerDto>?>> GetActive(BuyerDto model)
        {
            var returnData = new ResponseModel<IEnumerable<BuyerDto>?>();
            var proc = $"Usp_Buyer_GetActive";
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<BuyerDto>(proc);

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
