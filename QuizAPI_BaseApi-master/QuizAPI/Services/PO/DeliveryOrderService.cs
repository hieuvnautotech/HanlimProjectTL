using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.PO
{
    public interface IDeliveryOrderService
    {
        Task<ResponseModel<IEnumerable<DeliveryOrderDto>?>> Get(DeliveryOrderDto model);
        Task<ResponseModel<DeliveryOrderDto?>> GetById(long doId);
        Task<ResponseModel<IEnumerable<MaterialDto>?>> GetProductsByPO(long poId);
        Task<string> Create(DeliveryOrderDto model);
        Task<string> Modify(DeliveryOrderDto model);
        Task<string> DeleteReuse(DeliveryOrderDto model);
    }

    [ScopedRegistration]
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public DeliveryOrderService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<string> Create(DeliveryOrderDto model)
        {
            string proc = "Usp_DeliveryOrder_Create";
            var param = new DynamicParameters();
            param.Add("@DoId", model.DoId);
            param.Add("@DoCode", model.DoCode);
            param.Add("@PoId", model.PoId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@OrderQty", model.OrderQty);
            param.Add("@PackingNote", model.PackingNote);
            param.Add("@InvoiceNo", model.InvoiceNo);
            param.Add("@Dock", model.Dock);
            param.Add("@ETDLoad", model.ETDLoad);
            param.Add("@DeliveryTime", model.DeliveryTime);
            param.Add("@Remark", model.Remark);
            param.Add("@Truck", model.Truck);

            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> DeleteReuse(DeliveryOrderDto model)
        {
            string proc = "Usp_DeliveryOrder_DeleteReuse";
            var param = new DynamicParameters();
            param.Add("@DoId", model.DoId);
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@isActived", model.isActived);
            param.Add("@row_version", model.row_version);

            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<DeliveryOrderDto>?>> Get(DeliveryOrderDto model)
        {
            var returnData = new ResponseModel<IEnumerable<DeliveryOrderDto>?>();
            var proc = $"Usp_DeliveryOrder_Get";
            var param = new DynamicParameters();
            param.Add("@page", model.page);
            param.Add("@pageSize", model.pageSize);
            param.Add("@DoCode", model.DoCode);
            param.Add("@PoId", model.PoId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@ETDLoad", model.ETDLoad);
            param.Add("@DeliveryTime", model.DeliveryTime);
            param.Add("@isActived", model.isActived);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<DeliveryOrderDto>(proc, param);

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

        public async Task<ResponseModel<DeliveryOrderDto?>> GetById(long doId)
        {
            var returnData = new ResponseModel<DeliveryOrderDto?>();
            string proc = "Usp_DeliveryOrder_GetById";
            var param = new DynamicParameters();
            param.Add("@DoId", doId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<DeliveryOrderDto>(proc, param);
            
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

        public async Task<ResponseModel<IEnumerable<MaterialDto>?>> GetProductsByPO(long poId)
        {
            var returnData = new ResponseModel<IEnumerable<MaterialDto>?>();
            string proc = "Usp_DeliveryOrder_GetProductsByPO";
            var param = new DynamicParameters();
            param.Add("@PoId", poId);
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
            //throw new NotImplementedException();
        }

        public async Task<string> Modify(DeliveryOrderDto model)
        {
            string proc = "Usp_DeliveryOrder_Modify";
            var param = new DynamicParameters();
            param.Add("@DoId", model.DoId);
            param.Add("@DoCode", model.DoCode);
            param.Add("@PoId", model.PoId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@OrderQty", model.OrderQty);
            param.Add("@PackingNote", model.PackingNote);
            param.Add("@InvoiceNo", model.InvoiceNo);
            param.Add("@Dock", model.Dock);
            param.Add("@ETDLoad", model.ETDLoad);
            param.Add("@DeliveryTime", model.DeliveryTime);
            param.Add("@Remark", model.Remark);
            param.Add("@Truck", model.Truck);

            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }
    }
}
