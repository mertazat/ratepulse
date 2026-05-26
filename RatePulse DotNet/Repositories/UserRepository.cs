using Google.Cloud.Firestore;
using RatePulse.Models.Firestore;

namespace RatePulse.Repositories;

public class UserRepository : IUserRepository
{
    private readonly FirestoreDb _db;
    private const string Collection = "users";

    public UserRepository(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<UserModel?> GetByUidAsync(string uid)
    {
        var doc = await _db.Collection(Collection).Document(uid).GetSnapshotAsync();
        if (!doc.Exists) return null;
        return MapFromDoc(doc, uid);
    }

    public async Task<List<UserModel>> GetAllAsync()
    {
        var snapshot = await _db.Collection(Collection).GetSnapshotAsync();
        return snapshot.Documents.Select(d => MapFromDoc(d, d.Id)).ToList();
    }

    public async Task CreateAsync(UserModel user)
    {
        var dict = new Dictionary<string, object>
        {
            ["email"] = user.Email,
            ["displayName"] = user.DisplayName,
            ["fcmToken"] = user.FcmToken,
            ["role"] = user.Role,
            ["permissions"] = user.Permissions,
            ["preferredCurrencies"] = user.PreferredCurrencies,
            ["isBanned"] = user.IsBanned,
            ["createdAt"] = Timestamp.FromDateTime(user.CreatedAt.ToUniversalTime()),
            ["lastLogin"] = Timestamp.FromDateTime(user.LastLogin.ToUniversalTime())
        };
        await _db.Collection(Collection).Document(user.Uid).SetAsync(dict);
    }

    public async Task UpdateAsync(UserModel user)
    {
        var updates = new Dictionary<string, object>
        {
            ["displayName"] = user.DisplayName,
            ["preferredCurrencies"] = user.PreferredCurrencies
        };
        await _db.Collection(Collection).Document(user.Uid).UpdateAsync(updates);
    }

    public async Task UpdateFcmTokenAsync(string uid, string fcmToken)
    {
        await _db.Collection(Collection).Document(uid).UpdateAsync(
            new Dictionary<string, object> { ["fcmToken"] = fcmToken });
    }

    public async Task UpdateRoleAsync(string uid, string role)
    {
        await _db.Collection(Collection).Document(uid).UpdateAsync(
            new Dictionary<string, object> { ["role"] = role });
    }

    public async Task UpdateBanStatusAsync(string uid, bool isBanned)
    {
        await _db.Collection(Collection).Document(uid).UpdateAsync(
            new Dictionary<string, object> { ["isBanned"] = isBanned });
    }

    public async Task DeleteAsync(string uid)
    {
        await _db.Collection(Collection).Document(uid).DeleteAsync();
    }

    public async Task UpdateLastLoginAsync(string uid)
    {
        await _db.Collection(Collection).Document(uid).UpdateAsync(
            new Dictionary<string, object>
            {
                ["lastLogin"] = Timestamp.FromDateTime(DateTime.UtcNow)
            });
    }

    private static UserModel MapFromDoc(DocumentSnapshot doc, string uid)
    {
        return new UserModel
        {
            Uid = uid,
            Email = doc.ContainsField("email") ? doc.GetValue<string>("email") : "",
            DisplayName = doc.ContainsField("displayName") ? doc.GetValue<string>("displayName") : "",
            FcmToken = doc.ContainsField("fcmToken") ? doc.GetValue<string>("fcmToken") : "",
            Role = doc.ContainsField("role") ? doc.GetValue<string>("role") : "user",
            Permissions = doc.ContainsField("permissions") ? doc.GetValue<List<string>>("permissions") : new(),
            PreferredCurrencies = doc.ContainsField("preferredCurrencies") ? doc.GetValue<List<string>>("preferredCurrencies") : new(),
            IsBanned = doc.ContainsField("isBanned") && doc.GetValue<bool>("isBanned"),
            CreatedAt = doc.ContainsField("createdAt") ? doc.GetValue<Timestamp>("createdAt").ToDateTime() : DateTime.UtcNow,
            LastLogin = doc.ContainsField("lastLogin") ? doc.GetValue<Timestamp>("lastLogin").ToDateTime() : DateTime.UtcNow,
        };
    }
}
