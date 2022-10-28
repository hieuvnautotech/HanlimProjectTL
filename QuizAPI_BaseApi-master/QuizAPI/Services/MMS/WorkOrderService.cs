using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Dtos;
using static QuizAPI.Extensions.ServiceExtensions;
using QuizAPI.DbAccess;
using Dapper;
using QuizAPI.Extensions;
using System.Data;

namespace QuizAPI.Services.MMS
{
    public interface IWorkOrderService
    {
        Task<ResponseModel<IEnumerable<WorkOrderDto>?>> Get(WorkOrderDto model);
        Task<ResponseModel<WorkOrderDto?>> GetById(long woId);
        Task<ResponseModel<IEnumerable<MaterialDto>?>> GetProductsByForecastPOMaster(long fPoMasterId);
        Task<string> Create(WorkOrderDto model);
        Task<string> Modify(WorkOrderDto model);
        Task<string> DeleteReuse(WorkOrderDto model);
    }

    [ScopedRegistration]
    public class WorkOrderService : IWorkOrderService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public WorkOrderService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<string> Create(WorkOrderDto model)
        {
            throw new NotImplementedException();
        }

        public async Task<string> DeleteReuse(WorkOrderDto model)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseModel<IEnumerable<WorkOrderDto>?>> Get(WorkOrderDto model)
        {
            var returnData = new ResponseModel<IEnumerable<WorkOrderDto>?>();
            var proc = $"Usp_WorkOrder_Get";
            var param = new DynamicParameters();
            param.Add("@page", model.page);
            param.Add("@pageSize", model.pageSize);
            param.Add("@isActived", model.isActived);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

            param.Add("@WoCode", model.WoCode);
            param.Add("@FPoMasterId", model.FPoMasterId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@LineId", model.LineId);
            param.Add("@StartSearchingDate", model.StartSearchingDate);
            param.Add("@EndSearchingDate", model.EndSearchingDate);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<WorkOrderDto>(proc, param);

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

        public async Task<ResponseModel<WorkOrderDto?>> GetById(long woId)
        {
            var returnData = new ResponseModel<WorkOrderDto?>();
            string proc = "Usp_WorkOrder_GetById";
            var param = new DynamicParameters();
            param.Add("@WoId", woId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<WorkOrderDto>(proc, param);

            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            else
            {
                returnData.Data = data.FirstOrDefault();
                returnData.ResponseMessage = StaticReturnValue.SUCCESS;
            }
            return returnData;
        }

        public async Task<ResponseModel<IEnumerable<MaterialDto>?>> GetProductsByForecastPOMaster(long fPoMasterId)
        {
            var returnData = new ResponseModel<IEnumerable<MaterialDto>?>();
            string proc = "Usp_DeliveryOrder_GetProductsByPOMaster";
            var param = new DynamicParameters();
            param.Add("@FPoMasterId", fPoMasterId);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MaterialDto>(proc, param);
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            else
            {
                returnData.Data = data;
            }
            return returnData;
        }

        public async Task<string> Modify(WorkOrderDto model)
        {
            throw new NotImplementedException();
        }
    }
}
