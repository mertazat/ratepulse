using RatePulse.Services.Alert;
using RatePulse.Services.Currency;

namespace RatePulse.Services.Background;

public class CurrencyBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<CurrencyBackgroundService> _logger;

    public CurrencyBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration config,
        ILogger<CurrencyBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CurrencyBackgroundService started.");

        await RunFetchCycleAsync();

        int intervalMinutes = _config.GetValue<int>("CurrencyFetch:IntervalMinutes", 60);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);

            if (!stoppingToken.IsCancellationRequested)
                await RunFetchCycleAsync();
        }
    }

    private async Task RunFetchCycleAsync()
    {
        using var scope = _scopeFactory.CreateScope();

        var fetchService = scope.ServiceProvider.GetRequiredService<ICurrencyFetchService>();
        var alertService = scope.ServiceProvider.GetRequiredService<IAlertService>();

        _logger.LogInformation("Fetching currency rates at {Time}", DateTime.UtcNow);

        var rates = await fetchService.FetchAndSaveLatestRatesAsync();

        if (rates != null)
            await alertService.CheckAndTriggerAlertsAsync(rates.Values);
    }
}
