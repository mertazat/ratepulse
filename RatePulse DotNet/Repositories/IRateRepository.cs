using RatePulse.Models.Firestore;

namespace RatePulse.Repositories;

public interface IRateRepository
{
    Task<RateModel?> GetLatestAsync();
    Task<List<RateModel>> GetHistoryAsync(int limit = 48);
    Task SaveAsync(RateModel rate);
}
