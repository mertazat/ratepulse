using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RatePulse.Models.DTOs;
using RatePulse.Models.Firestore;
using RatePulse.Repositories;

namespace RatePulse.Controllers.API;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertRepository _alertRepository;

    public AlertsController(IAlertRepository alertRepository)
    {
        _alertRepository = alertRepository;
    }

    private string GetUid() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet]
    public async Task<IActionResult> GetAlerts()
    {
        var alerts = await _alertRepository.GetByUserIdAsync(GetUid());
        var dtos = alerts.Select(a => new AlertResponseDto
        {
            Id = a.Id,
            FromCurrency = a.FromCurrency,
            ToCurrency = a.ToCurrency,
            TargetRate = a.TargetRate,
            Direction = a.Direction,
            IsActive = a.IsActive,
            TriggeredAt = a.TriggeredAt,
            CreatedAt = a.CreatedAt
        }).ToList();

        return Ok(ApiResponse<List<AlertResponseDto>>.Ok(dtos));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDto dto)
    {
        var alert = new AlertModel
        {
            UserId = GetUid(),
            FromCurrency = dto.FromCurrency.ToUpper(),
            ToCurrency = dto.ToCurrency.ToUpper(),
            TargetRate = dto.TargetRate,
            Direction = dto.Direction,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var id = await _alertRepository.CreateAsync(alert);
        return Ok(ApiResponse.Ok($"Alert created with id: {id}"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAlert(string id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        if (alert == null)
            return NotFound(ApiResponse.Fail("Alert not found"));

        if (alert.UserId != GetUid())
            return Forbid();

        await _alertRepository.DeleteAsync(id);
        return Ok(ApiResponse.Ok("Alert deleted"));
    }
}
