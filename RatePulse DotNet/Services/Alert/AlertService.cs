using RatePulse.Repositories;
using RatePulse.Services.Notification;

namespace RatePulse.Services.Alert;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<AlertService> _logger;

    public AlertService(
        IAlertRepository alertRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<AlertService> logger)
    {
        _alertRepository = alertRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task CheckAndTriggerAlertsAsync(Dictionary<string, double> currentRates)
    {
        var activeAlerts = await _alertRepository.GetAllActiveAsync();

        foreach (var alert in activeAlerts)
        {
            try
            {
                double currentRate = GetCrossRate(currentRates, alert.FromCurrency, alert.ToCurrency);

                bool triggered = alert.Direction == "above"
                    ? currentRate >= alert.TargetRate
                    : currentRate <= alert.TargetRate;

                if (!triggered) continue;

                await _alertRepository.DeactivateAsync(alert.Id, DateTime.UtcNow);

                var user = await _userRepository.GetByUidAsync(alert.UserId);
                if (user == null || string.IsNullOrEmpty(user.FcmToken)) continue;

                string title = "Kur Alarmı Tetiklendi!";
                string body = $"{alert.FromCurrency}/{alert.ToCurrency} kuru {currentRate:F4} seviyesine ulaştı. Hedef: {alert.TargetRate:F4}";

                await _notificationService.SendToUserAsync(
                    user.FcmToken, title, body,
                    new Dictionary<string, string>
                    {
                        ["alertId"] = alert.Id,
                        ["fromCurrency"] = alert.FromCurrency,
                        ["toCurrency"] = alert.ToCurrency,
                        ["currentRate"] = currentRate.ToString("F4"),
                        ["targetRate"] = alert.TargetRate.ToString("F4")
                    });

                _logger.LogInformation("Alert {Id} triggered for user {Uid}", alert.Id, alert.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing alert {Id}", alert.Id);
            }
        }
    }

    private static double GetCrossRate(Dictionary<string, double> usdRates, string from, string to)
    {
        if (from == "USD") return usdRates.TryGetValue(to, out var rate) ? rate : 0;
        if (to == "USD") return usdRates.TryGetValue(from, out var fromRate) ? 1.0 / fromRate : 0;

        if (!usdRates.TryGetValue(from, out var fromUsd)) return 0;
        if (!usdRates.TryGetValue(to, out var toUsd)) return 0;

        return toUsd / fromUsd;
    }
}
