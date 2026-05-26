using Microsoft.AspNetCore.Mvc;
using RatePulse.Models.DTOs;
using RatePulse.Models.Firestore;
using RatePulse.Repositories;
using RatePulse.Services.Auth;

namespace RatePulse.Controllers.API;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IFirebaseAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AuthController(IFirebaseAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        var firebaseToken = await _authService.VerifyTokenAsync(token);
        if (firebaseToken == null)
            return Unauthorized(ApiResponse.Fail("Invalid token"));

        var existing = await _userRepository.GetByUidAsync(firebaseToken.Uid);
        if (existing != null)
        {
            await _userRepository.UpdateFcmTokenAsync(firebaseToken.Uid, dto.FcmToken);
            await _userRepository.UpdateLastLoginAsync(firebaseToken.Uid);
            return Ok(ApiResponse.Ok("User already exists, FCM token updated"));
        }

        var user = new UserModel
        {
            Uid = firebaseToken.Uid,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            FcmToken = dto.FcmToken,
            Role = "user",
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);
        return Ok(ApiResponse.Ok("User registered successfully"));
    }
}
