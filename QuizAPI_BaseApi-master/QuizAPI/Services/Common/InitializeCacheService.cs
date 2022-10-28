using Microsoft.Extensions.Caching.Memory;

namespace QuizAPI.Services.Common
{
    public class InitializeCacheService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserAuthorizationService _userAuthorizationService;
        private readonly IRefreshTokenService _refreshTokenService;
        public InitializeCacheService(IServiceProvider serviceProvider
            , IUserAuthorizationService userAuthorizationService
         , IRefreshTokenService refreshTokenService
         )
        {
            _serviceProvider = serviceProvider;
            _userAuthorizationService = userAuthorizationService;
            _refreshTokenService = refreshTokenService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var cache = _serviceProvider.GetService<IMemoryCache>();

                var userAuthorization = _userAuthorizationService.GetUserAuthorization();
                var availableTokens = _refreshTokenService.GetAvailables();

                cache.Set("userAuthorization", userAuthorization.Result);
                cache.Set("availableTokens", availableTokens.Result);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
