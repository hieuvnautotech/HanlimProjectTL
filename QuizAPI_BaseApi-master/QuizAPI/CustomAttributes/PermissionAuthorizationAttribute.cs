using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;

namespace QuizAPI.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PermissionAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        private readonly HashSet<string> _permissionHash = new();
        public PermissionAuthorizationAttribute(params string[] permissions)
        {
            //_roleList = string.Join(",", roles);
            if (permissions.Length > 0)
            {
                foreach (var item in permissions)
                {
                    if (!_permissionHash.Add(item))
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
                    //ResponseMessage = "Unauthorized",
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
                if (!_permissionHash.Any())
                {
                    return;
                }
                else
                {
                    var _memoryCache = context.HttpContext.RequestServices.GetService(typeof(IMemoryCache)) as IMemoryCache;

                    _memoryCache.TryGetValue("userAuthorization", out IEnumerable<UserAuthorizationDto> userAuthorizationList);
                    _memoryCache.TryGetValue("availableTokens", out HashSet<string> availableTokens);

                    var permissionHash = new HashSet<string>();
                    var permissionList = userAuthorizationList.Where(x => x.userId == Int64.Parse(userId)).ToList();

                    foreach (var item in permissionList)
                    {
                        if (!permissionHash.Add(item.permissionName))
                        {
                            continue;
                        }
                    }

                    if (permissionHash.Any(_permissionHash.Contains) && availableTokens.Contains(token))
                    {
                        return;
                    }
                    else
                    {
                        if (!availableTokens.Contains(token))
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
