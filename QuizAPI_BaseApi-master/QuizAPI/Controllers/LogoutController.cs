using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.Extensions;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;

namespace QuizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogoutController : ControllerBase
    {
        private readonly ILogoutService _logoutService;
        private readonly IMemoryCache _memoryCache;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserAuthorizationService _userAuthorizationService;

        public LogoutController(ILogoutService logoutService, IMemoryCache memoryCache, IRefreshTokenService refreshTokenService, IUserAuthorizationService userAuthorizationService)
        {
            _logoutService = logoutService;
            _memoryCache = memoryCache;
            _refreshTokenService = refreshTokenService;
            _userAuthorizationService = userAuthorizationService;
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var returnData = new ResponseModel<bool>();
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != "logout_token")
            {
                
                var result = await _logoutService.LogoutAsync(token);
                returnData.ResponseMessage = result;
                switch (result)
                {
                    case StaticReturnValue.SUCCESS:
                        var availableTokens = _refreshTokenService.GetAvailables();
                        _memoryCache.Remove("availableTokens");
                        _memoryCache.Set("availableTokens", availableTokens.Result);

                        var userAuthorization = _userAuthorizationService.GetUserAuthorization();
                        _memoryCache.Remove("userAuthorization");
                        _memoryCache.Set("userAuthorization", userAuthorization.Result);

                        returnData.Data = true;
                        break;
                    default:
                        returnData.HttpResponseCode = 500;
                        returnData.Data = false;
                        break;
                }
            }
            else
            {
                returnData.ResponseMessage = StaticReturnValue.SUCCESS;
                returnData.Data=true;
            }

            return Ok(returnData);
        }
    }
}
