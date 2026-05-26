using Microsoft.AspNetCore.Mvc;
using RatePulse.Models.Firestore;
using RatePulse.Repositories;
using RatePulse.Services.Auth;

namespace RatePulse.Controllers.Admin;

[Route("admin")]
public class AdminAuthController : Controller
{
    private readonly IFirebaseAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AdminAuthController(IFirebaseAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        if (!string.IsNullOrEmpty(HttpContext.Session.GetString("AdminUid")))
            return RedirectToAction("Index", "AdminDashboard");

        return View();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] string idToken)
    {
        var firebaseToken = await _authService.VerifyTokenAsync(idToken);
        if (firebaseToken == null)
        {
            ViewBag.Error = "Geçersiz token. Lütfen tekrar deneyin.";
            return View();
        }

        var user = await _userRepository.GetByUidAsync(firebaseToken.Uid);

        if (user == null)
        {
            var email = firebaseToken.Claims.TryGetValue("email", out var emailClaim)
                ? emailClaim.ToString()! : firebaseToken.Uid;
            user = new UserModel
            {
                Uid = firebaseToken.Uid,
                Email = email,
                DisplayName = email.Contains('@') ? email.Split('@')[0] : email,
                FcmToken = "",
                Role = "user",
                Permissions = new(),
                PreferredCurrencies = new(),
                IsBanned = false,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow
            };
            await _userRepository.CreateAsync(user);
            ViewBag.Error = "Hesabınız sisteme kaydedildi ancak admin yetkisi atanmamış. " +
                            "Firebase Console → Firestore → users → " + firebaseToken.Uid +
                            " belgesinde 'role' alanını 'superadmin' yapın, ardından tekrar giriş yapın.";
            return View();
        }

        if (user.Role != "admin" && user.Role != "superadmin")
        {
            ViewBag.Error = "Bu panele erişim yetkiniz yok. (Mevcut rol: " + user.Role + ")";
            return View();
        }

        if (user.IsBanned)
        {
            ViewBag.Error = "Hesabınız askıya alınmış.";
            return View();
        }

        HttpContext.Session.SetString("AdminUid", user.Uid);
        HttpContext.Session.SetString("AdminEmail", user.Email);
        HttpContext.Session.SetString("AdminRole", user.Role);
        HttpContext.Session.SetString("AdminName", user.DisplayName);

        await _userRepository.UpdateLastLoginAsync(user.Uid);

        return RedirectToAction("Index", "AdminDashboard");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
