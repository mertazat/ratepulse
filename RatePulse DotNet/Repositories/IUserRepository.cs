using RatePulse.Models.Firestore;

namespace RatePulse.Repositories;

public interface IUserRepository
{
    Task<UserModel?> GetByUidAsync(string uid);
    Task<List<UserModel>> GetAllAsync();
    Task CreateAsync(UserModel user);
    Task UpdateAsync(UserModel user);
    Task UpdateFcmTokenAsync(string uid, string fcmToken);
    Task UpdateRoleAsync(string uid, string role);
    Task UpdateBanStatusAsync(string uid, bool isBanned);
    Task DeleteAsync(string uid);
    Task UpdateLastLoginAsync(string uid);
}
