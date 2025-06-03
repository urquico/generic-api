using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApi.Services.Auth
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public RefreshTokenCleanupService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Calculate initial delay until next 8:00 AM UTC+8
            var now = DateTime.UtcNow.AddHours(8);
            var nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0, DateTimeKind.Utc);

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            var initialDelay = nextRun - now;
            if (initialDelay < TimeSpan.Zero)
            {
                initialDelay = TimeSpan.Zero;
            }

            await Task.Delay(initialDelay, stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();
                    tokenService.DeleteExpiredRefreshTokens();
                }

                // Wait 24 hours for the next run
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
}
