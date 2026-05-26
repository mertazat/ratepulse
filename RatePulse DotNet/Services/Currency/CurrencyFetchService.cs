using System.Text.Json;
using RatePulse.Models.Firestore;
using RatePulse.Repositories;

namespace RatePulse.Services.Currency;

public class CurrencyFetchService : ICurrencyFetchService
{
    private readonly HttpClient _httpClient;
    private readonly IRateRepository _rateRepository;
    private readonly IConfiguration _config;
    private readonly ILogger<CurrencyFetchService> _logger;

    public CurrencyFetchService(
        HttpClient httpClient,
        IRateRepository rateRepository,
        IConfiguration config,
        ILogger<CurrencyFetchService> logger)
    {
        _httpClient = httpClient;
        _rateRepository = rateRepository;
        _config = config;
        _logger = logger;
    }

    public async Task<RateModel?> FetchAndSaveLatestRatesAsync()
    {
        try
        {
            var apiKey = _config["FreeCurrencyApi:ApiKey"];
            var baseUrl = _config["FreeCurrencyApi:BaseUrl"];
            var currencies = string.Join(",", _config.GetSection("CurrencyFetch:Currencies").Get<string[]>() ?? Array.Empty<string>());

            var url = $"{baseUrl}/latest?apikey={apiKey}&currencies={currencies}";
            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var data = doc.RootElement.GetProperty("data");

            var values = new Dictionary<string, double>();
            foreach (var prop in data.EnumerateObject())
                values[prop.Name] = prop.Value.GetDouble();

            var rateModel = new RateModel
            {
                Base = "USD",
                FetchedAt = DateTime.UtcNow,
                Values = values
            };

            await _rateRepository.SaveAsync(rateModel);
            _logger.LogInformation("Rates fetched and saved at {Time}", DateTime.UtcNow);
            return rateModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch currency rates");
            return null;
        }
    }
}
