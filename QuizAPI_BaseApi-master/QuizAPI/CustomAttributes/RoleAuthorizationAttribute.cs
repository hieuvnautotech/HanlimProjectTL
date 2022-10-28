using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;

namespace QuizAPI.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        //private readonly List<string> _roleList;
        private readonly HashSet<string> _roleHash = new();
        public RoleAuthorizationAttribute(params string[] roles)
        {
            //_roleList = string.Join(",", roles);
            if (roles.Length > 0)
            {
                foreach (var item in roles)
                {
                    if (!_roleHash.Add(item))
                    {
                        continue;
                    }
                }
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAll] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAllAttribute>().Any();
            if (allowAnonymous) return;

            // authorization
            var userId = context.HttpContext.Items["UserId"]?.ToString();
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(userId))
            {

                context.Result = new JsonResult(new ResponseModel<UserDto>
                {
                    ResponseMessage = StaticReturnValue.LOST_AUTHORIZATION,
                    HttpResponseCode = 401,
                    Data = null
                })
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }
            else
            {
                if (!_roleHash.Any())
                {
                    return;
                }
                else
                {
                    var _memoryCache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

                    _memoryCache.TryGetValue("userAuthorization", out IEnumerable<UserAuthorizationDto> userAuthorizationList);
                    _memoryCache.TryGetValue("availableTokens", out HashSet<string> availableTokens);

                    var roleHash = new HashSet<string>();
                    var roleList = userAuthorizationList.Where(x => x.userId == Int64.Parse(userId)).ToList();

                    foreach (var item in roleList)
                    {
                        if (!roleHash.Add(item.roleName))
                        {
                            continue;
                        }
                    }

                    if (roleHash.Any(_roleHash.Contains) && availableTokens.Contains(token))
                    {
                        return;
                    }
                    else
                    {
                        context.Result = new JsonResult(new ResponseModel<UserDto>
                        {
                            ResponseMessage = StaticReturnValue.UNAUTHORIZED,
                            HttpResponseCode = 401,
                            Data = null
                        })
                        {
                            StatusCode = StatusCodes.Status200OK
                        };
                    }
                }
            }
        }
    }
}
