using RatePulse.Models.Firestore;

namespace RatePulse.Repositories;

public interface IAlertRepository
{
    Task<AlertModel?> GetByIdAsync(string id);
    Task<List<AlertModel>> GetByUserIdAsync(string userId);
    Task<List<AlertModel>> GetAllActiveAsync();
    Task<List<AlertModel>> GetAllAsync();
    Task<string> CreateAsync(AlertModel alert);
    Task UpdateAsync(AlertModel alert);
    Task DeleteAsync(string id);
    Task DeactivateAsync(string id, DateTime triggeredAt);
}
