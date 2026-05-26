namespace RatePulse.Services.Alert;

public interface IAlertService
{
    Task CheckAndTriggerAlertsAsync(Dictionary<string, double> currentRates);
}
