using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using challenge1.Common.EmailService.Classes.Interfaces;

namespace challenge1.Common.EmailService.Services
{
    public class EmailBackgroundService : BackgroundService
    {
        private readonly IBackgroundEmailQueue _queue;
        private readonly ILogger<EmailBackgroundService> _logger;

        public EmailBackgroundService(IBackgroundEmailQueue queue, ILogger<EmailBackgroundService> logger)
        {
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(stoppingToken);
                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending email");
                }
            }
        }
    }
}
