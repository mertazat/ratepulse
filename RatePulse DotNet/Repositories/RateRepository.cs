using Google.Cloud.Firestore;
using RatePulse.Models.Firestore;

namespace RatePulse.Repositories;

public class RateRepository : IRateRepository
{
    private readonly FirestoreDb _db;
    private const string Collection = "rates";

    public RateRepository(FirestoreDb db)
    {
        _db = db;
    }

    public async Task<RateModel?> GetLatestAsync()
    {
        var snapshot = await _db.Collection(Collection)
            .OrderByDescending("fetchedAt")
            .Limit(1)
            .GetSnapshotAsync();

        if (snapshot.Count == 0) return null;
        return MapFromDoc(snapshot.Documents[0]);
    }

    public async Task<List<RateModel>> GetHistoryAsync(int limit = 48)
    {
        var snapshot = await _db.Collection(Collection)
            .OrderByDescending("fetchedAt")
            .Limit(limit)
            .GetSnapshotAsync();

        return snapshot.Documents.Select(MapFromDoc).ToList();
    }

    public async Task SaveAsync(RateModel rate)
    {
        var id = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var dict = new Dictionary<string, object>
        {
            ["base"] = rate.Base,
            ["fetchedAt"] = Timestamp.FromDateTime(DateTime.UtcNow),
            ["values"] = rate.Values.ToDictionary(k => k.Key, v => (object)v.Value)
        };
        await _db.Collection(Collection).Document(id).SetAsync(dict);
        rate.Id = id;
    }

    private static RateModel MapFromDoc(DocumentSnapshot doc)
    {
        var values = new Dictionary<string, double>();
        if (doc.ContainsField("values"))
        {
            var rawValues = doc.GetValue<Dictionary<string, object>>("values");
            foreach (var kv in rawValues)
                values[kv.Key] = Convert.ToDouble(kv.Value);
        }

        return new RateModel
        {
            Id = doc.Id,
            Base = doc.ContainsField("base") ? doc.GetValue<string>("base") : "USD",
            FetchedAt = doc.ContainsField("fetchedAt") ? doc.GetValue<Timestamp>("fetchedAt").ToDateTime() : DateTime.UtcNow,
            Values = values
        };
    }
}
