using FirebaseAdmin.Auth;

namespace RatePulse.Services.Auth;

public class FirebaseAuthService : IFirebaseAuthService
{
    private readonly ILogger<FirebaseAuthService> _logger;

    public FirebaseAuthService(ILogger<FirebaseAuthService> logger)
    {
        _logger = logger;
    }

    public async Task<FirebaseToken?> VerifyTokenAsync(string idToken)
    {
        try
        {
            return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Firebase token verification failed: {Message}", ex.Message);
            return null;
        }
    }
}
