using Dapper;
using QuizAPI.DbAccess;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models;
using QuizAPI.Models.Dtos.Common;
using System.Data;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface IMenuService
    {
        Task<ResponseModel<IEnumerable<MenuDto>?>> GetAll(MenuDto model, string? keyWord);
        Task<ResponseModel<IEnumerable<MenuDto>?>> GetExceptRoot(MenuDto model, string keyWord);
        Task<ResponseModel<IEnumerable<MenuDto>?>> GetByRole(PageModel pageInfo, long roleId,string keyWord);
        Task<ResponseModel<MenuDto?>> GetById(long menuId);
        Task<string> Create(MenuDto model);
        Task<string> Modify(MenuDto model);
        Task<string> Delete(MenuDto model);
        Task<ResponseModel<IEnumerable<MenuDto>?>> GetByLevel(byte menuLevel);
        Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect();
    }

    [ScopedRegistration]
    public class MenuService : IMenuService
    {
        private readonly ISqlDataAccess _sqlDataAccess;

        public MenuService(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public async Task<string> Create(MenuDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_Menu()))
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            model.menuName = model.menuName?.Trim();
            model.menuComponent = model.menuComponent?.Trim().Replace(" ", "");
            if (!string.IsNullOrWhiteSpace(model.navigateUrl))
            {
                model.navigateUrl = model.navigateUrl?.Trim().Replace(" ", "");
            }
            else
            {
                model.navigateUrl = null;
            }
            
            if (model.menuLevel == 2 && (model.parentId == null || model.parentId == 0))
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            //foreach (System.Reflection.PropertyInfo propertyInfo in tempObj.GetType().GetProperties())
            //{
            //    if (propertyInfo == null)
            //    {
            //        return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            //    }
            //}

            string proc = "sysUsp_Menu_Create";
            var param = new DynamicParameters();
            param.Add("@menuId", model.menuId);
            param.Add("@parentId", model.parentId);
            param.Add("@menuName", model.menuName);
            param.Add("@menuIcon", model.menuIcon);
            param.Add("@languageKey", model.languageKey);
            param.Add("@menuComponent", model.menuComponent);
            param.Add("@navigateUrl", model.navigateUrl);
            param.Add("@forRoot", model.forRoot);
            param.Add("@forApp", model.forApp);
            param.Add("@menuLevel", model.menuLevel);
            param.Add("@sortOrder", model.sortOrder);
            param.Add("@createdBy", model.createdBy);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            //throw new NotImplementedException();
        }

        public async Task<ResponseModel<IEnumerable<MenuDto>?>> GetAll(MenuDto model, string? keyWord)
        {
            var returnData = new ResponseModel<IEnumerable<MenuDto>?>();
            var proc = $"sysUsp_Menu_GetAll";
            var param = new DynamicParameters();
            param.Add("@page", model.page);
            param.Add("@pageSize", model.pageSize);
            param.Add("@keyWord", keyWord);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MenuDto>(proc, param);
            
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

            //throw new NotImplementedException();
        }

        public async Task<ResponseModel<IEnumerable<MenuDto>?>> GetExceptRoot(MenuDto model, string keyWord)
        {
            var returnData = new ResponseModel<IEnumerable<MenuDto>?>();
            var proc = $"sysUsp_Menu_GetExceptRoot";
            var param = new DynamicParameters();
            param.Add("@page", model.page);
            param.Add("@pageSize", model.pageSize);
            param.Add("@keyWord", keyWord);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MenuDto>(proc, param);
            
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

            //throw new NotImplementedException();
        }

        public async Task<ResponseModel<IEnumerable<MenuDto>?>> GetByRole(PageModel pageInfo, long roleId,string keyWord)
        {
            var returnData = new ResponseModel<IEnumerable<MenuDto>?>();
            string proc = "sysUsp_Menu_GetByRole";
            var param = new DynamicParameters();
            param.Add("@roleId", roleId);
            param.Add("@keyword", keyWord);
            param.Add("@page", pageInfo.page);
            param.Add("@pageSize", pageInfo.pageSize);
            param.Add("@totalRow", 0, DbType.Int32, ParameterDirection.Output);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MenuDto>(proc, param);
            returnData.Data = data;
            returnData.TotalRow = param.Get<int>("totalRow");
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;
        }

        public async Task<ResponseModel<MenuDto?>> GetById(long menuId)
        {
            var returnData = new ResponseModel<MenuDto?>();
            string proc = "sysUsp_Menu_GetById";
            var param = new DynamicParameters();
            param.Add("@menuId", menuId);

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MenuDto>(proc, param);
            returnData.Data = data.FirstOrDefault();
            if (!data.Any())
            {
                returnData.HttpResponseCode = 204;
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
            }
            return returnData;

            //throw new NotImplementedException();
        }

        public async Task<string> Modify(MenuDto model)
        {
            if (!ValidateModel.CheckValid(model, new sysTbl_Menu()))
            {
                return StaticReturnValue.OBJECT_INVALID; //Object Invalid
            }

            if (string.IsNullOrWhiteSpace(model.navigateUrl))
            {
                model.navigateUrl = null;
            }
            if (string.IsNullOrWhiteSpace(model.menuComponent))
            {
                model.menuComponent = null;
            }

            string proc = "sysUsp_Menu_Modify";
            var param = new DynamicParameters();
            param.Add("@menuId", model.menuId);
            param.Add("@parentId", model.parentId);
            param.Add("@menuName", model.menuName);
            param.Add("@menuLevel", model.menuLevel);
            param.Add("@sortOrder", model.sortOrder);
            param.Add("@menuIcon", model.menuIcon);
            param.Add("@languageKey", model.languageKey);
            param.Add("@menuComponent", model.menuComponent);
            param.Add("@navigateUrl", model.navigateUrl) ;
            param.Add("@forRoot", model.forRoot);
            param.Add("@forApp", model.forApp);
            param.Add("@modifyBy", model.modifiedBy);
            param.Add("@row_version", model.row_version);
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);

            //throw new NotImplementedException();
        }

        private MenuTreeDto CreateTreeMenuNode(List<MenuDto> List, MenuDto Menu)
        {
            MenuTreeDto TreeNode = new()
            {
                menuId = Menu.menuId,
                parentId = Menu.parentId ,
                menuName = Menu.menuName,
                menuComponent = Menu.menuComponent,
                navigateUrl = Menu.navigateUrl,
                menuIcon = Menu.menuIcon,
                languageKey = Menu.languageKey,
                subMenus = new List<MenuTreeDto>()
            };

            foreach (var item in List)
            {
                if (item.parentId == Menu.menuId)
                {
                    var SubChild = CreateTreeMenuNode(List, item);
                    TreeNode.subMenus.Add(SubChild);
                }
            }

            return TreeNode;
        }

        public async Task<ResponseModel<IEnumerable<MenuDto>?>> GetByLevel(byte menuLevel)
        {
            var returnData = new ResponseModel<IEnumerable<MenuDto>?>();
            var proc = $"sysUsp_Menu_GetByLevel";
            var param = new DynamicParameters();
            param.Add("@menuLevel", menuLevel);
            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<MenuDto>(proc, param);
            returnData.Data = data;
            if (!data.Any())
            {
                returnData.ResponseMessage = StaticReturnValue.NO_DATA;
                returnData.HttpResponseCode = 204;
            }

            return returnData;
        }

        public async Task<string> Delete(MenuDto model)
        {
            string proc = "sysUsp_Menu_Delete";
            var param = new DynamicParameters();
            param.Add("@menuId", model.menuId);
            
            param.Add("@output", dbType: DbType.String, direction: ParameterDirection.Output, size: int.MaxValue);//luôn để DataOutput trong stored procedure

            return await _sqlDataAccess.SaveDataUsingStoredProcedure<int>(proc, param);
        }

        public async Task<ResponseModel<IEnumerable<dynamic>?>> GetForSelect()
        {
            var returnData = new ResponseModel<IEnumerable<dynamic>?>();
            var proc = $"sysUsp_Menu_GetForSelect";
            var param = new DynamicParameters();

            var data = await _sqlDataAccess.LoadDataUsingStoredProcedure<dynamic>(proc);
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
