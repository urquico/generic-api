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
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var tokenService = scope.ServiceProvider.GetRequiredService<TokenService>();
                    tokenService.DeleteExpiredRefreshTokens();
                }
                // run every 8:00AM
                var now = DateTime.UtcNow.AddHours(8); // Adjust to your local time zone if necessary
                var nextRun = new DateTime(now.Year, now.Month, now.Day, 8, 0, 0, DateTimeKind.Utc);

                if (now > nextRun)
                {
                    nextRun = nextRun.AddDays(1);
                }

                var delay = nextRun - now;

                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.Zero;
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
