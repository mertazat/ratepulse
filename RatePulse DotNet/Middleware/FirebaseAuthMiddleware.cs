using System.Security.Claims;
using RatePulse.Services.Auth;

namespace RatePulse.Middleware;

public class FirebaseAuthMiddleware
{
    private readonly RequestDelegate _next;

    public FirebaseAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IFirebaseAuthService authService)
    {
        var path = context.Request.Path.Value ?? "";

        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        if (path.StartsWith("/api/auth/", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Authorization header missing" });
            return;
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();
        var firebaseToken = await authService.VerifyTokenAsync(token);

        if (firebaseToken == null)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsJsonAsync(new { success = false, message = "Invalid or expired token" });
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, firebaseToken.Uid),
            new(ClaimTypes.Email, firebaseToken.Claims.TryGetValue("email", out var email) ? email.ToString()! : "")
        };
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Firebase"));

        await _next(context);
    }
}
