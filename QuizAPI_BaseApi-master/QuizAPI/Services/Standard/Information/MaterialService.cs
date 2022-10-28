using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models;
using QuizAPI.Models.Dtos;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Models.Validators;
using QuizAPI.Services.Base;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common.Standard.Information
{
    public interface IMaterialService
    {
        Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? MaterialType, long? Unit, long? SupplierId, bool showDelete);
        Task<ResponseModel<MaterialDto?>> GetById(long? MaterialId);
        Task<ResponseModel<MaterialDto?>> Create(MaterialDto model);
        Task<ResponseModel<MaterialDto?>> Modify(MaterialDto model);
        Task<ResponseModel<MaterialDto?>> Delete(long? MaterialId, byte[] row_version, long user);
        Task<ResponseModel<IEnumerable<MaterialDto>>> GetForBom(long? BomId, int BomLV);
    }
    [ScopedRegistration]
    public class MaterialService : IMaterialService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public MaterialService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetAll(PageModel pageInfo, string keyWord, long? MaterialType, long? Unit, long? SupplierId, bool showDelete = true)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<dynamic>?>();
                string proc = "Usp_Material_GetAll"; var param = new DynamicParameters();
                param.Add("@keyword", keyWord);
                param.Add("@MaterialType", MaterialType);
                param.Add("@Unit", Unit);
                param.Add("@SupplierId", SupplierId);
                param.Add("@showDelete", showDelete);
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<dynamic>(proc, param);
                returnData.Data = data;
                returnData.TotalRow = param.Get<int>("totalRow");
                if (!data.Any())
                {
                    returnData.HttpResponseCode = 204;
                    returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                }
                return returnData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseModel<MaterialDto?>> Create(MaterialDto model)
        {
            var returnData = new ResponseModel<MaterialDto?>();

            var validator = new MaterialValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Material_Create";
            var param = new DynamicParameters();
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@MaterialCode", model.MaterialCode);
            param.Add("@Description", model.Description);
            param.Add("@MaterialType", model.MaterialType);
            param.Add("@Unit", model.Unit);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var result = await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            returnData.ResponseMessage = result;
            switch (result)
            {
                case StaticReturnValue.SYSTEM_ERROR:
                    returnData.HttpResponseCode = 500;
                    break;
                case StaticReturnValue.SUCCESS:
                    if (model.Suppliers != null)
                        await AddSupplier(model);
                    returnData = await GetById(model.MaterialId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<MaterialDto?>> Modify(MaterialDto model)
        {
            var returnData = new ResponseModel<MaterialDto?>();

            var validator = new MaterialValidator();
            var validateResults = validator.Validate(model);
            if (!validateResults.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = validateResults.Errors[0].ToString();
                return returnData;
            }

            string proc = "Usp_Material_Modify";
            var param = new DynamicParameters();
            param.Add("@MaterialId", model.MaterialId);
            param.Add("@MaterialCode", model.MaterialCode);
            param.Add("@Description", model.Description);
            param.Add("@MaterialType", model.MaterialType);
            param.Add("@Unit", model.Unit);
            param.Add("@createdBy", model.createdBy);
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
                    if (model.Suppliers != null)
                        await AddSupplier(model);
                    returnData = await GetById(model.MaterialId);
                    returnData.ResponseMessage = result;
                    break;
                default:
                    returnData.HttpResponseCode = 400;
                    break;
            }
            return returnData;
        }

        public async Task<ResponseModel<MaterialDto?>> GetById(long? MaterialId)
        {
            var returnData = new ResponseModel<MaterialDto?>();
            string proc = "Usp_Material_GetById";
            var param = new DynamicParameters();
            param.Add("@MaterialId", MaterialId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MaterialDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = "NO DATA";
            }
            return returnData;
        }

        public async Task<ResponseModel<MaterialDto?>> Delete(long? MaterialId, byte[] row_version, long user)
        {
            string proc = "Usp_Material_Delete";
            var param = new DynamicParameters();
            param.Add("@MaterialId", MaterialId);
            param.Add("@row_version", row_version);
            param.Add("@createdBy", user);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            var returnData = new ResponseModel<MaterialDto?>();
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

        public async Task<string> AddSupplier(MaterialDto model)
        {
            try
            {
                var SupplierIds = new List<long>();
                foreach (var item in model.Suppliers)
                {
                    SupplierIds.Add(item.SupplierId);
                }

                string proc = "Usp_Material_AddSupplier";
                var param = new DynamicParameters();
                param.Add("@MaterialId", model.MaterialId);
                param.Add("@SupplierIds", ParameterTvp.GetTableValuedParameter_BigInt(SupplierIds));
                param.Add("@modifiedBy", model.createdBy);
                param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

                return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ResponseModel<IEnumerable<MaterialDto>>> GetForBom(long? BomId, int BomLV)
        {
            var returnData = new ResponseModel<IEnumerable<MaterialDto>>();
            var proc = $"Usp_Material_GetForBom";
            var param = new DynamicParameters();
            param.Add("@BomId", BomId);
            param.Add("@BomLV", BomLV);

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
