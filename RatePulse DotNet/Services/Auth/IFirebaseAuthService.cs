using FirebaseAdmin.Auth;

namespace RatePulse.Services.Auth;

public interface IFirebaseAuthService
{
    Task<FirebaseToken?> VerifyTokenAsync(string idToken);
}
