using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RatePulse.Models.DTOs;
using RatePulse.Models.Firestore;
using RatePulse.Repositories;

namespace RatePulse.Controllers.API;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    private string GetUid() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
    private string GetEmail() => User.FindFirstValue(ClaimTypes.Email) ?? "";

    private async Task<UserModel> GetOrCreateUserAsync()
    {
        var uid = GetUid();
        var user = await _userRepository.GetByUidAsync(uid);
        if (user != null) return user;

        var email = GetEmail();
        var newUser = new UserModel
        {
            Uid = uid,
            Email = email,
            DisplayName = email.Contains('@') ? email.Split('@')[0] : uid,
            FcmToken = "",
            Role = "user",
            Permissions = new(),
            PreferredCurrencies = new(),
            IsBanned = false,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
        await _userRepository.CreateAsync(newUser);
        return newUser;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var user = await GetOrCreateUserAsync();

        return Ok(ApiResponse<UserProfileDto>.Ok(new UserProfileDto
        {
            Uid = user.Uid,
            Email = user.Email,
            DisplayName = user.DisplayName,
            PreferredCurrencies = user.PreferredCurrencies,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        }));
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var user = await GetOrCreateUserAsync();

        user.DisplayName = dto.DisplayName;
        user.PreferredCurrencies = dto.PreferredCurrencies;
        await _userRepository.UpdateAsync(user);

        return Ok(ApiResponse.Ok("Profile updated"));
    }

    [HttpPut("fcm-token")]
    public async Task<IActionResult> UpdateFcmToken([FromBody] UpdateFcmTokenDto dto)
    {
        await _userRepository.UpdateFcmTokenAsync(GetUid(), dto.FcmToken);
        return Ok(ApiResponse.Ok("FCM token updated"));
    }
}
