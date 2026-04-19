using challenge1.Common.Logging;
using challenge1.Common.Util.WeatherClients.Interfaces;
using challenge1.Database.Models.Weather;
using challenge1.Database.Repositories.Repositories.Weather.Interfaces;

namespace challenge1.Application.API.BackgroundServices
{
    /// <summary>
    /// Runs every 15 minutes: checks latest weather records against active alert subscriptions and sends email notifications when thresholds are breached.
    /// </summary>
    public class WeatherAlertBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(15);

        public WeatherAlertBackgroundService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAlertsAsync(stoppingToken);
                }
                catch (Exception)
                {
                    // Errors logged inside CheckAlertsAsync per alert
                }

                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task CheckAlertsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var logging = scope.ServiceProvider.GetRequiredService<ILogging>();
            var alertRepo = scope.ServiceProvider.GetRequiredService<IWeatherAlertRepository>();
            var recordRepo = scope.ServiceProvider.GetRequiredService<IWeatherRecordRepository>();

            var activeAlerts = await alertRepo.GetActiveAlertsForCheckAsync();
            if (activeAlerts == null || activeAlerts.Count == 0) return;

            foreach (var alert in activeAlerts)
            {
                if (stoppingToken.IsCancellationRequested) break;

                try
                {
                    var latestRecord = await recordRepo.GetLatestByLocationAsync(alert.LocationId);
                    if (latestRecord == null) continue;

                    if (IsThresholdBreached(alert, latestRecord))
                    {
                        await SendAlertNotificationAsync(scope, alert, latestRecord, logging);

                        var alertToUpdate = await alertRepo.GetByIdAsync(alert.AlertId);
                        if (alertToUpdate != null)
                        {
                            alertToUpdate.LastTriggeredAt = DateTime.Now;
                            await alertRepo.UpdateAsync(alertToUpdate);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logging.LogError($"Alert check failed for AlertId={alert.AlertId}: {ex}");
                }
            }
        }

        private static bool IsThresholdBreached(WeatherAlert alert, WeatherRecord record)
        {
            return alert.AlertType switch
            {
                "temperature_high" => record.Temperature.HasValue && record.Temperature.Value >= alert.ThresholdValue,
                "temperature_low"  => record.Temperature.HasValue && record.Temperature.Value <= alert.ThresholdValue,
                "rainfall"         => record.Rainfall.HasValue && record.Rainfall.Value >= alert.ThresholdValue,
                "aqi"              => record.AirQualityIndex.HasValue && record.AirQualityIndex.Value >= (int)alert.ThresholdValue,
                _                  => false
            };
        }

        private async Task SendAlertNotificationAsync(IServiceScope scope, WeatherAlert alert, WeatherRecord record, ILogging logging)
        {
            try
            {
                var emailServiceType = Type.GetType("challenge1.Common.EmailService.Classes.Interfaces.IEmailService, Challenge.Common.EmailService");
                if (emailServiceType == null) return;

                var emailService = scope.ServiceProvider.GetService(emailServiceType);
                if (emailService == null) return;

                var subject = $"[Weather Alert] {alert.AlertType.Replace('_', ' ').ToUpper()} threshold breached";
                var body = BuildAlertEmailBody(alert, record);

                var method = emailServiceType.GetMethod("SendEmailAsync", [typeof(string), typeof(string), typeof(string)]);
                if (method != null)
                    await (Task)method.Invoke(emailService, [alert.SubscriberEmail, subject, body])!;
            }
            catch (Exception ex)
            {
                logging.LogError($"Failed to send alert email for AlertId={alert.AlertId}: {ex}");
            }
        }

        private static string BuildAlertEmailBody(WeatherAlert alert, WeatherRecord record)
        {
            return $"""
                <html><body>
                <p>Dear {alert.SubscriberName},</p>
                <p>A weather alert has been triggered for your subscription.</p>
                <table border="1" cellpadding="6" cellspacing="0">
                  <tr><td><b>Alert Type</b></td><td>{alert.AlertType}</td></tr>
                  <tr><td><b>Threshold</b></td><td>{alert.ThresholdValue}</td></tr>
                  <tr><td><b>Recorded At</b></td><td>{record.RecordedAt:yyyy-MM-dd HH:mm} UTC</td></tr>
                  <tr><td><b>Temperature</b></td><td>{record.Temperature?.ToString("F1") ?? "N/A"} °C</td></tr>
                  <tr><td><b>Humidity</b></td><td>{record.Humidity?.ToString("F1") ?? "N/A"} %</td></tr>
                  <tr><td><b>Rainfall</b></td><td>{record.Rainfall?.ToString("F1") ?? "N/A"} mm</td></tr>
                  <tr><td><b>Air Quality Index</b></td><td>{record.AirQualityIndex?.ToString() ?? "N/A"}</td></tr>
                  <tr><td><b>Description</b></td><td>{record.WeatherDescription ?? "N/A"}</td></tr>
                </table>
                <p>You can unsubscribe at any time.</p>
                </body></html>
                """;
        }
    }
}
