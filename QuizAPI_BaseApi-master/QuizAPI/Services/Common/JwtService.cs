using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using QuizAPI.DbAccess;
using QuizAPI.Models.Dtos.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static QuizAPI.Extensions.ServiceExtensions;

namespace QuizAPI.Services.Common
{
    public interface IJwtService
    {
        Task<AuthorizationResponse> GetTokenAsync(UserDto model);
        string? ValidateToken(string token);
        long GetUserIdFromToken(string token);
        string GetUserNameFromToken(string token);
        string? GetRolename(string token);
    }

    [ScopedRegistration]
    public class JwtService : IJwtService
    {

        private readonly IMemoryCache _memoryCache;

        public JwtService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<AuthorizationResponse> GetTokenAsync(UserDto model)
        {
            var token = await GenerateToken(model);
            var refreshToken = await GenerateRefreshToken();
            return new AuthorizationResponse { accessToken = token, refreshToken = refreshToken };
        }

        private async Task<string?> GenerateToken(UserDto model)
        {
            //List<string> roles = new();
            //if (model.RoleNames.Any())
            //{
            //    foreach (var item in model.RoleNames)
            //    {
            //        roles.Add(item);
            //    }

            //    int claimsLength = roles.Count + 1;
            //    var claims = new Claim[claimsLength];

            //    claims[0] = new Claim(ClaimTypes.NameIdentifier, model.UserId.ToString());

            //    for (int i = 1; i < claimsLength; i++)
            //    {
            //        claims[i] = new Claim(ClaimTypes.Role, roles[i - 1]);
            //    }

            //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConnectionString.SECRET));

            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var descriptor = new SecurityTokenDescriptor()
            //    {
            //        Subject = new ClaimsIdentity(claims),
            //        Expires = DateTime.UtcNow.AddMinutes(90),
            //        SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
            //        Audience = ConnectionString.AUDIENCE,
            //        Issuer = ConnectionString.ISSUER
            //    };

            //    //var cres = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //    //var descriptor = new JwtSecurityToken(
            //    //    claims: claims,
            //    //    expires: DateTime.UtcNow.AddDays(1),
            //    //    signingCredentials: cres
            //    //    );

            //    var token = tokenHandler.CreateToken(descriptor);
            //    return await Task.FromResult(tokenHandler.WriteToken(token)); 
            //}
            var claims = new Claim[3];
            claims[0] = new Claim(ClaimTypes.NameIdentifier, model.userId.ToString());
            claims[1] = new Claim(ClaimTypes.Name, model.userName);
            claims[2] = new Claim(ClaimTypes.Role, model.RoleNameList);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConnectionString.SECRET));

            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(480),
                //Expires = DateTime.UtcNow.AddSeconds(15),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature),
                Audience = ConnectionString.AUDIENCE,
                Issuer = ConnectionString.ISSUER
            };

            var token = tokenHandler.CreateToken(descriptor);
            return await Task.FromResult(tokenHandler.WriteToken(token));
        }

        public string? ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return String.Empty;
            }

            _memoryCache.TryGetValue("availableTokens", out HashSet<string?> availableTokens);

            if (!availableTokens.Contains(token))
            {
                return String.Empty;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConnectionString.SECRET));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateIssuer = true,
                    ValidIssuer = ConnectionString.ISSUER,

                    ValidateAudience = true,
                    ValidAudience = ConnectionString.AUDIENCE,

                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value;

                return userId;
            }
            catch
            {
                return string.Empty;
                throw;
            }
        }

        private static async Task<string> GenerateRefreshToken()
        {
            var byteArr = new byte[64];
            using var cryptoProvider = RandomNumberGenerator.Create();
            cryptoProvider.GetBytes(byteArr);
            return await Task.FromResult(Convert.ToBase64String(byteArr));
        }

        public long GetUserIdFromToken(string token)
        {
            if (long.TryParse(ValidateToken(token), out long userId))
                return userId;
            else
                return 0;
        }

        public string GetUserNameFromToken(string token)
        {
            return GetUsername(token) ?? string.Empty;
        }

        private string? GetUsername(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return String.Empty;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConnectionString.SECRET));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateIssuer = true,
                    ValidIssuer = ConnectionString.ISSUER,

                    ValidateAudience = true,
                    ValidAudience = ConnectionString.AUDIENCE,

                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userName = jwtToken.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value;

                return userName;
            }
            catch
            {
                return string.Empty;
                throw;
            }
        }

        public string? GetRolename(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return String.Empty;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConnectionString.SECRET));
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,

                    ValidateIssuer = true,
                    ValidIssuer = ConnectionString.ISSUER,

                    ValidateAudience = true,
                    ValidAudience = ConnectionString.AUDIENCE,

                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userName = jwtToken.Claims.FirstOrDefault(x => x.Type == "role")?.Value;

                return userName;
            }
            catch
            {
                return string.Empty;
                throw;
            }
        }
    }
}
