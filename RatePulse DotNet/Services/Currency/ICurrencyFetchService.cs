using RatePulse.Models.Firestore;

namespace RatePulse.Services.Currency;

public interface ICurrencyFetchService
{
    Task<RateModel?> FetchAndSaveLatestRatesAsync();
}
