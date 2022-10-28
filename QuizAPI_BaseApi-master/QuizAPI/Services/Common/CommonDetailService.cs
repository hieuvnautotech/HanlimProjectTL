using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Models;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Base;
using System.Data;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface ICommonDetailService
    {
        Task<ResponseModel<IEnumerable<CommonDetailDto>?>> GetByCommonMasterId(PageModel pageInfo, long userId, long commonMasterId);
        Task<ResponseModel<IEnumerable<CommonDetailDto>?>> GetPermissionType();
        Task<string> Create(CommonDetailDto model);
        Task<ResponseModel<CommonDetailDto?>> GetById(long id);
        Task<string> Modify(CommonDetailDto model);
        Task<string> Delete(long commonDetailId);
    }
    [ScopedRegistration]
    public class CommonDetailService : ICommonDetailService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public CommonDetailService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<ResponseModel<IEnumerable<CommonDetailDto>?>> GetByCommonMasterId(PageModel pageInfo, long userId, long commonMasterId)
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<CommonDetailDto>?>();
                string proc = "sysUsp_CommonDetail_GetByCommonMasterId";
                var param = new DynamicParameters();
                param.Add("@page", pageInfo.page);
                param.Add("@pageSize", pageInfo.pageSize);
                param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@commonMasterId", commonMasterId);

                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<CommonDetailDto>(proc, param);
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

        public async Task<string> Create(CommonDetailDto model)
        {
            var commonDetail = AutoMapperConfig<CommonDetailDto, sysTbl_CommonDetail>.Map(model);
            var isValid = SimpleValidator.IsModelValid(commonDetail);
            if (!isValid || model == null)
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            string proc = "sysUsp_CommonDetail_Create";
            var param = new DynamicParameters();
            param.Add("@commonMasterId", model.commonMasterId);
            param.Add("@commonDetailId", model.commonDetailId);
            param.Add("@commonDetailName", model.commonDetailName?.ToUpper());
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<string> Delete(long commonDetailId)
        {
            string proc = "sysUsp_CommonDetail_Delete";
            var param = new DynamicParameters();
            param.Add("@commonDetailId", commonDetailId);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<CommonDetailDto?>> GetById(long id)
        {
            var returnData = new ResponseModel<CommonDetailDto?>();
            string proc = "sysUsp_CommonDetail_GetById";
            var param = new DynamicParameters();
            param.Add("@commonDetailId", id);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<CommonDetailDto?>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<string> Modify(CommonDetailDto model)
        {
            var commonDetail = AutoMapperConfig<CommonDetailDto, sysTbl_CommonDetail>.Map(model);
            var isValid = SimpleValidator.IsModelValid(commonDetail);
            if (!isValid || model == null)
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            string proc = "sysUsp_CommonDetail_Modify";
            var param = new DynamicParameters();
            param.Add("@commonDetailId", model.commonDetailId);
            param.Add("@commonDetailName", model.commonDetailName?.ToUpper());
            param.Add("@modifiedBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<string>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<CommonDetailDto>?>> GetPermissionType()
        {
            try
            {
                var returnData = new ResponseModel<IEnumerable<CommonDetailDto>?>();
                string proc = "sysUsp_CommonDetail_GetPermissionType";
                var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<CommonDetailDto>(proc);
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
            catch (Exception)
            {

                throw;
            }
        }
    }
}
