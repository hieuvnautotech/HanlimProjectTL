using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.CustomAttributes;
using QuizAPI.Extensions;
using QuizAPI.Helpers;
using QuizAPI.Models.Dtos.Common;
using QuizAPI.Services.Common;
using System.IdentityModel.Tokens.Jwt;

namespace QuizAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefreshTokenController : ControllerBase
    {
        private readonly IJwtService _jwtService;
        private readonly IUserService _userService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMemoryCache _memoryCache;

        public RefreshTokenController(IJwtService jwtService, IUserService userService, IRefreshTokenService refreshTokenService, IMemoryCache memoryCache)
        {
            _jwtService = jwtService;
            _userService = userService;
            _refreshTokenService = refreshTokenService;
            _memoryCache = memoryCache;
        }

        [HttpPost]
        [AllowAll]
        public async Task<IActionResult> RefreshToken([FromBody] UserRefreshTokenRequest request)
        {
            var returnData = new ResponseModel<AuthorizationResponse>();
            if (!ModelState.IsValid)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = StaticReturnValue.OBJECT_INVALID;
                return Ok(returnData);
            }

            var token = GetJwtToken(request.expiredToken);
            request.ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            request.ipAddress = UserIPHelper.UserIp;
            var refreshToken = await _refreshTokenService.Get(request);

            var authorizationResponse = ValidateToken(token, refreshToken);
            if (!authorizationResponse.isSuccess)
            {
                returnData.HttpResponseCode = 400;
                returnData.ResponseMessage = authorizationResponse.reason;
                return Ok(returnData);
            }

            //var updateRefreshTokenResult = await _userRefreshTokenService.Update(userRefreshToken);
            //if (updateRefreshTokenResult == 1)
            //{
            var userId = token.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            var user = await _userService.GetByUserId(long.Parse(userId));
            if (user == null)
            {
                returnData.HttpResponseCode = 403;
                returnData.ResponseMessage = StaticReturnValue.USER_NOTFOUND;
                return Ok(returnData);
                //return Forbid(StaticReturnValue.USERINFO_NOTFOUND_STR);
            }
            var authResponse = await _jwtService.GetTokenAsync(user.Data);
            //user.Data.Token = authResponse.Token;

            RefreshTokenDto refreshTokenDto = new()
            {
                accessToken = authResponse.accessToken,
                refreshToken = authResponse.refreshToken,
                createdDate = DateTime.UtcNow,
                expiredDate = refreshToken.expiredDate,
                //IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                //IpAddress = HttpContext.Connection.LocalIpAddress?.ToString(),
                //IpAddress = HttpContext.Request.Headers["X-Forwarded-For"].ToString(),
                ipAddress = UserIPHelper.UserIp,
                isValidated = false,
                userId = user.Data.userId,
            };

            var result = await _refreshTokenService.Create(refreshTokenDto);
            if (result == StaticReturnValue.SUCCESS)
            {
                var availableTokens = _refreshTokenService.GetAvailables();
                _memoryCache.Remove("availableTokens");
                _memoryCache.Set("availableTokens", availableTokens.Result);
                returnData.Data = authResponse;
                returnData.ResponseMessage = StaticReturnValue.SUCCESS;
                return Ok(returnData);
            }
            else
            {
                returnData.HttpResponseCode = 500;
                returnData.ResponseMessage = StaticReturnValue.SYSTEM_ERROR;
                return Ok(returnData);
            }
            //}

            //returnData.HttpResponseCode = 500;
            //returnData.ResponseMessage = StaticReturnValue.SYSTEM_ERROR_STR;
            //return Ok(returnData);
        }

        private JwtSecurityToken GetJwtToken(string expiredToken)
        {
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            return jwtSecurityTokenHandler.ReadJwtToken(expiredToken);
        }

        private AuthorizationResponse ValidateToken(JwtSecurityToken token, RefreshTokenDto refreshToken)
        {
            if (refreshToken == null)
            {
                return new AuthorizationResponse { isSuccess = false, reason = "Invalid Refresh Token !" };
            }

            if (refreshToken.isActive == false)
            {
                return new AuthorizationResponse { isSuccess = false, reason = "Refresh Token expired !" };
            }

            if (token.ValidTo > DateTime.UtcNow)
            {
                return new AuthorizationResponse { isSuccess = false, reason = "Token is not expired !" };
            }

            return new AuthorizationResponse ();
        }
    }
}
