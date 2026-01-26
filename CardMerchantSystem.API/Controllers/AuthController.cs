using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Services;
using CardMerchantSystem.Shared.Kernel.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace CardMerchantSystem.API.Controllers;


[EnableRateLimiting("Strict")]
public class AuthController : ApiControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Kullanıcı girişi
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        return Ok(HandleNotFound(response, "Geçersiz kullanıcı adı veya şifre"));
    }

    /// <summary>
    /// Yeni kullanıcı oluşturur (Sadece Admin)
    /// </summary>
    [HttpPost("register")]
    [Authorize(Policy = Policies.AdminOnly)]
    public async Task<ActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.CreateUserAsync(
            request.Username,
            request.Email,
            request.Password,
            request.FullName,
            request.Roles);

        if (!result)
            throw new ConflictException("Kullanıcı oluşturulamadı. Kullanıcı adı veya email zaten mevcut.");

        return Ok(new { message = "Kullanıcı başarıyla oluşturuldu" });
    }

    /// <summary>
    /// Mevcut kullanıcı bilgisi
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            throw new UnauthorizedException();

        var user = await _authService.GetUserByUsernameAsync(username);

        return Ok(HandleNotFound(user, "Kullanıcı"));
    }

    /// <summary>
    /// Mevcut rolleri listeler
    /// </summary>
    [HttpGet("roles")]
    [Authorize(Policy = Policies.AdminOnly)]
    public ActionResult GetRoles()
    {
        var roles = new[]
        {
            new { Id = 1, Name = "Admin", DisplayName = "Sistem Yöneticisi" },
            new { Id = 2, Name = "CardOperator", DisplayName = "Kart Operasyon" },
            new { Id = 3, Name = "MerchantOperator", DisplayName = "Üye İşyeri Operasyon" },
            new { Id = 4, Name = "FinanceOperator", DisplayName = "Finans Operasyon" },
            new { Id = 5, Name = "ComplianceOfficer", DisplayName = "Uyum Sorumlusu" },
            new { Id = 6, Name = "CallCenterAgent", DisplayName = "Çağrı Merkezi" },
            new { Id = 7, Name = "Viewer", DisplayName = "Görüntüleyici" }
        };

        return Ok(roles);
    }
}