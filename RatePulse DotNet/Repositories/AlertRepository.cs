using Google.Cloud.Firestore;
using RatePulse.Models.Firestore;

namespace RatePulse.Repositories;

public class AlertRepository : IAlertRepository
{
    private readonly FirestoreDb _db;
    private const string Collection = "alerts";

    public AlertRepository(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<AlertModel?> GetByIdAsync(string id)
    {
        var doc = await _db.Collection(Collection).Document(id).GetSnapshotAsync();
        if (!doc.Exists) return null;
        return MapFromDoc(doc);
    }

    public async Task<List<AlertModel>> GetByUserIdAsync(string userId)
    {
        var snapshot = await _db.Collection(Collection)
            .WhereEqualTo("userId", userId)
            .OrderByDescending("createdAt")
            .GetSnapshotAsync();
        return snapshot.Documents.Select(MapFromDoc).ToList();
    }

    public async Task<List<AlertModel>> GetAllActiveAsync()
    {
        var snapshot = await _db.Collection(Collection)
            .WhereEqualTo("isActive", true)
            .GetSnapshotAsync();
        return snapshot.Documents.Select(MapFromDoc).ToList();
    }

    public async Task<List<AlertModel>> GetAllAsync()
    {
        var snapshot = await _db.Collection(Collection)
            .OrderByDescending("createdAt")
            .GetSnapshotAsync();
        return snapshot.Documents.Select(MapFromDoc).ToList();
    }

    public async Task<string> CreateAsync(AlertModel alert)
    {
        var dict = new Dictionary<string, object>
        {
            ["userId"] = alert.UserId,
            ["fromCurrency"] = alert.FromCurrency,
            ["toCurrency"] = alert.ToCurrency,
            ["targetRate"] = alert.TargetRate,
            ["direction"] = alert.Direction,
            ["isActive"] = alert.IsActive,
            ["createdAt"] = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        var docRef = await _db.Collection(Collection).AddAsync(dict);
        return docRef.Id;
    }

    public async Task UpdateAsync(AlertModel alert)
    {
        var updates = new Dictionary<string, object>
        {
            ["targetRate"] = alert.TargetRate,
            ["direction"] = alert.Direction,
            ["isActive"] = alert.IsActive
        };
        await _db.Collection(Collection).Document(alert.Id).UpdateAsync(updates);
    }

    public async Task DeleteAsync(string id)
    {
        await _db.Collection(Collection).Document(id).DeleteAsync();
    }

    public async Task DeactivateAsync(string id, DateTime triggeredAt)
    {
        await _db.Collection(Collection).Document(id).UpdateAsync(
            new Dictionary<string, object>
            {
                ["isActive"] = false,
                ["triggeredAt"] = Timestamp.FromDateTime(triggeredAt.ToUniversalTime())
            });
    }

    private static AlertModel MapFromDoc(DocumentSnapshot doc)
    {
        return new AlertModel
        {
            Id = doc.Id,
            UserId = doc.GetValue<string>("userId"),
            FromCurrency = doc.GetValue<string>("fromCurrency"),
            ToCurrency = doc.GetValue<string>("toCurrency"),
            TargetRate = doc.GetValue<double>("targetRate"),
            Direction = doc.GetValue<string>("direction"),
            IsActive = doc.GetValue<bool>("isActive"),
            TriggeredAt = doc.ContainsField("triggeredAt") ? doc.GetValue<Timestamp>("triggeredAt").ToDateTime() : null,
            CreatedAt = doc.ContainsField("createdAt") ? doc.GetValue<Timestamp>("createdAt").ToDateTime() : DateTime.UtcNow,
        };
    }
}
