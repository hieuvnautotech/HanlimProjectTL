using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Dtos;
using static QuizAPI.Extensions.ServiceExtensions;
using QuizAPI.DbAccess;
using Dapper;
using QuizAPI.Extensions;
using System.Data;
using QuizAPI.Models.Validators;
using Microsoft.VisualBasic;
using QuizAPI.Models;

namespace QuizAPI.Services.PO
{
    public interface IPurchaseOrderService
    {
        Task<ResponseModel<IEnumerable<PurchaseOrderDto>?>> Get(PurchaseOrderDto model);
        Task<ResponseModel<PurchaseOrderDto?>> Create(PurchaseOrderDto model);
        Task<ResponseModel<PurchaseOrderDto?>> Modify(PurchaseOrderDto model);
        Task<ResponseModel<PurchaseOrderDto?>> GetById(long? PoId);
        Task<ResponseModel<PurchaseOrderDto?>> Delete(PurchaseOrderDto model);
        Task<ResponseModel<IEnumerable<PODetailDto>?>> GetDetail(BaseModel model, long PoId);
        Task<ResponseModel<PODetailDto?>> CreateDetail(PODetailDto model);
        Task<ResponseModel<PODetailDto?>> ModifyDetail(PODetailDto model);
        Task<ResponseModel<PODetailDto?>> GetDetailById(long? PoId);
        Task<ResponseModel<PODetailDto?>> DeleteDetail(PODetailDto model);
        Task<ResponseModel<IEnumerable<MaterialDto>>> GetMaterial(long PoId);
        Task<ResponseModel<IEnumerable<PODetailDto>?>> GetForReport(PurchaseOrderDto model);
    }

    [ScopedRegistration]
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        public PurchaseOrderService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        // PurchaseOrder
        public async Task<ResponseModel<IEnumerable<PurchaseOrderDto>?>> Get(PurchaseOrderDto model)
        {
            var returnData = new ResponseModel<IEnumerable<PurchaseOrderDto>?>();
            var proc = $"Usp_PurchaseOrder_Get";
            var param = new DynamicParameters();
            param.Add("@page", model.page);
            param.Add("@pageSize", model.pageSize);
            param.Add("@PoCode", model.PoCode);
            param.Add("@DeliveryDate", model.DeliveryDate);
            param.Add("@DueDate", model.DueDate);
            param.Add("@isActived", model.isActived);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PurchaseOrderDto>(proc, param);

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

        public async Task<ResponseModel<IEnumerable<PODetailDto>?>> GetForReport(PurchaseOrderDto model)
        {
            var returnData = new ResponseModel<IEnumerable<PODetailDto>?>();
            var proc = $"Usp_PurchaseOrder_GetDetail";
            var param = new DynamicParameters();
            param.Add("@PoCode", model.PoCode);
            param.Add("@DeliveryDate", model.DeliveryDate);
            param.Add("@DueDate", model.DueDate);
            param.Add("@isActived", model.isActived);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PODetailDto>(proc, param);

            returnData.Data = data;

            return returnData;
        }

        public async Task<ResponseModel<PurchaseOrderDto?>> Create(PurchaseOrderDto model)
        {
            var returnData = new ResponseModel<PurchaseOrderDto?>();

            var validator = new PurchaseOrderValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            //DateTime DeliveryDate = ((DateTime)model.DeliveryDate).ToUniversalTime();
            //DateTime DueDate = ((DateTime)model.DueDate).ToUniversalTime();

            string proc = "Usp_PurchaseOrder_Create";
            var param = new DynamicParameters();
            param.Add("@PoId", model.PoId);
            param.Add("@PoCode", model.PoCode);
            param.Add("@Description", model.Description);
            param.Add("@DeliveryDate", model.DeliveryDate);
            param.Add("@DueDate", model.DueDate);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetById(model.PoId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<PurchaseOrderDto?>> Modify(PurchaseOrderDto model)
        {
            var returnData = new ResponseModel<PurchaseOrderDto?>();

            var validator = new PurchaseOrderValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_PurchaseOrder_Modify";
            var param = new DynamicParameters();
            param.Add("@PoId", model.PoId);
            param.Add("@PoCode", model.PoCode);
            param.Add("@Description", model.Description);
            param.Add("@DeliveryDate", model.DeliveryDate);
            param.Add("@DueDate", model.DueDate);
            param.Add("@modifiedBy", model.createdBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.REFRESH_REQUIRED:
                    returnData.HttpResponseCode = 500;
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetById(model.PoId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<PurchaseOrderDto?>> GetById(long? PoId)
        {
            var returnData = new ResponseModel<PurchaseOrderDto?>();
            string proc = "Usp_PurchaseOrder_GetById";
            var param = new DynamicParameters();
            param.Add("@PoId", PoId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PurchaseOrderDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<PurchaseOrderDto?>> Delete(PurchaseOrderDto model)
        {
            string proc = "Usp_PurchaseOrder_Delete";
            var param = new DynamicParameters();
            param.Add("@PoId", model.PoId);
            param.Add("@row_version", model.row_version);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<PurchaseOrderDto?>();
            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.REFRESH_REQUIRED:
                    returnData.HttpResponseCode = 500;
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SUCCESS:
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return returnData;
        }

        // PO detail
        public async Task<ResponseModel<IEnumerable<PODetailDto>?>> GetDetail(BaseModel model, long PoId)
        {
            var returnData = new ResponseModel<IEnumerable<PODetailDto>?>();
            var proc = $"Usp_PODetail_GetByPoId";
            var param = new DynamicParameters();
            param.Add("@PoId", PoId);
            param.Add("@isActived", model.isActived);
            param.Add("@page", model.page);
            param.Add("@pageSize", model.pageSize);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PODetailDto>(proc, param);

            if (data == null)
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

        public async Task<ResponseModel<PODetailDto?>> CreateDetail(PODetailDto model)
        {
            var returnData = new ResponseModel<PODetailDto?>();

            var validator = new PODetailValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            //DateTime DeliveryDate = (DateTime)model.DeliveryDate;
            //DateTime DueDate = (DateTime)model.DueDate;

            string proc = "Usp_PODetail_Create";
            var param = new DynamicParameters();
            param.Add("@PoDetailId", model.PoDetailId);
            param.Add("@PoId", model.PoId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@Description", model.Description);
            param.Add("@Qty", model.Qty);
            param.Add("@DeliveryDate", model.DeliveryDate);
            param.Add("@DueDate", model.DueDate);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetDetailById(model.PoDetailId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<PODetailDto?>> ModifyDetail(PODetailDto model)
        {
            var returnData = new ResponseModel<PODetailDto?>();

            var validator = new PODetailValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            //DateTime DeliveryDate = (DateTime)model.DeliveryDate;
            //DateTime DueDate = (DateTime)model.DueDate;

            string proc = "Usp_PODetail_Modify";
            var param = new DynamicParameters();
            param.Add("@PoDetailId", model.PoDetailId);
            param.Add("@PoId", model.PoId);
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@Description", model.Description);
            param.Add("@Qty", model.Qty);
            param.Add("@DeliveryDate", model.DeliveryDate);
            param.Add("@DueDate", model.DueDate);
            param.Add("@modifiedBy", model.createdBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.REFRESH_REQUIRED:
                    returnData.HttpResponseCode = 500;
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SUCCESS:
                    returnData = await GetDetailById(model.PoDetailId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<PODetailDto?>> GetDetailById(long? PoDetailId)
        {
            var returnData = new ResponseModel<PODetailDto?>();
            string proc = "Usp_PODetail_GetById";
            var param = new DynamicParameters();
            param.Add("@PoDetailId", PoDetailId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<PODetailDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<PODetailDto?>> DeleteDetail(PODetailDto model)
        {
            string proc = "Usp_PODetail_Delete";
            var param = new DynamicParameters();
            param.Add("@PoDetailId", model.PoDetailId);
            param.Add("@row_version", model.row_version);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);

            var returnData = new ResponseModel<PODetailDto?>();
            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.REFRESH_REQUIRED:
                    returnData.HttpResponseCode = 500;
                    returnData.ResponseMessage = result;
                    break;
                case StaticReturnValue.SUCCESS:
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }

            return returnData;
        }

        public async Task<ResponseModel<IEnumerable<MaterialDto>>> GetMaterial(long PoId)
        {
            var returnData = new ResponseModel<IEnumerable<MaterialDto>>();
            var proc = $"Usp_PODetail_GetMaterialForSelect";
            var param = new DynamicParameters();
            param.Add("@PoId", PoId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MaterialDto>(proc, param);
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
