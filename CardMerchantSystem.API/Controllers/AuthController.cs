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
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public AuthController(
        IAuthService authService,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _authService = authService;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Kullanıcı girişi (Local veya Keycloak proxy)
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var authProvider = _configuration["AuthProvider"] ?? "Local";

        if (authProvider == "Keycloak")
            return await KeycloakLoginAsync(request);

        var response = await _authService.LoginAsync(request);
        return Ok(HandleNotFound(response, "Geçersiz kullanıcı adı veya şifre"));
    }

    /// <summary>
    /// Token yenileme (Keycloak Refresh Token)
    /// Mevcut refresh token ile yeni access + refresh token alır.
    /// Frontend token expire olmadan önce bu endpoint'i çağırmalı.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var authProvider = _configuration["AuthProvider"] ?? "Local";

        if (authProvider != "Keycloak")
            return BadRequest(new { message = "Refresh token sadece Keycloak modunda desteklenir" });

        var client = _httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";

        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = _configuration["Keycloak:WebClientId"]!,
            ["refresh_token"] = request.RefreshToken
        };

        var httpResponse = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));

        if (!httpResponse.IsSuccessStatusCode)
        {
            var error = await httpResponse.Content.ReadAsStringAsync();

            // Refresh token expired veya invalid ise kullanıcı tekrar login olmalı
            return Unauthorized(new
            {
                message = "Refresh token geçersiz veya süresi dolmuş. Lütfen tekrar giriş yapın.",
                code = "REFRESH_TOKEN_EXPIRED"
            });
        }

        var json = await httpResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

        // Token'dan sub al → Admin API ile kullanıcı bilgilerini getir
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(json!.AccessToken);
        var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        string username = "", email = "", fullName = "";
        var roles = new List<string>();

        if (!string.IsNullOrEmpty(sub))
        {
            var adminClient = HttpContext.RequestServices.GetRequiredService<KeycloakAdminClient>();
            var keycloakUser = await adminClient.GetUserByIdAsync(sub);

            if (keycloakUser != null)
            {
                username = keycloakUser.Username;
                email = keycloakUser.Email ?? "";
                fullName = keycloakUser.FullName;
                roles = keycloakUser.Roles;
            }
        }

        // Fallback: roller token'dan
        if (!roles.Any())
        {
            var realmAccess = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            if (realmAccess != null)
            {
                using var doc = System.Text.Json.JsonDocument.Parse(realmAccess);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    roles = rolesElement.EnumerateArray()
                        .Select(r => r.GetString()!)
                        .Where(r => r is not "offline_access" and not "uma_authorization" and not "default-roles-cardmerchant")
                        .ToList();
                }
            }
        }

        return Ok(new LoginResponse
        {
            Token = json.AccessToken,
            RefreshToken = json.RefreshToken,
            Username = username,
            Email = email,
            FullName = string.IsNullOrEmpty(fullName) ? username : fullName,
            Roles = roles,
            ExpiresAt = DateTime.UtcNow.AddSeconds(json.ExpiresIn)
        });
    }

    /// <summary>
    /// Kullanıcı çıkışı (Keycloak SSO session sonlandırma)
    /// Hem access token hem refresh token geçersiz kılınır.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout([FromBody] LogoutRequest request)
    {
        var authProvider = _configuration["AuthProvider"] ?? "Local";

        if (authProvider != "Keycloak")
            return Ok(new { message = "Çıkış yapıldı" }); // Local modda sadece frontend token siler

        var client = _httpClientFactory.CreateClient();
        var logoutEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/logout";

        var logoutRequest = new Dictionary<string, string>
        {
            ["client_id"] = _configuration["Keycloak:WebClientId"]!,
            ["refresh_token"] = request.RefreshToken
        };

        var httpResponse = await client.PostAsync(logoutEndpoint, new FormUrlEncodedContent(logoutRequest));

        if (httpResponse.IsSuccessStatusCode)
        {
            return Ok(new { message = "Çıkış yapıldı. Keycloak session sonlandırıldı." });
        }

        // Logout başarısız olsa bile frontend token'ı silmeli
        return Ok(new { message = "Çıkış yapıldı. (Keycloak session sonlandırılamadı)" });
    }

    /// <summary>
    /// Keycloak üzerinden token alır ve frontend'in beklediği formata dönüştürür
    /// </summary>
    private async Task<ActionResult<LoginResponse>> KeycloakLoginAsync(LoginRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        var tokenEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/token";

        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _configuration["Keycloak:WebClientId"]!,
            ["username"] = request.Username,
            ["password"] = request.Password,
            ["scope"] = "openid profile email roles"
        };

        var httpResponse = await client.PostAsync(tokenEndpoint, new FormUrlEncodedContent(tokenRequest));

        if (!httpResponse.IsSuccessStatusCode)
            return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre" });

        var json = await httpResponse.Content.ReadFromJsonAsync<KeycloakTokenResponse>();

        // Token'dan sub al → Admin API ile kullanıcı bilgilerini getir
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(json!.AccessToken);
        var sub = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        string username = request.Username, email = "", fullName = "";
        var roles = new List<string>();

        if (!string.IsNullOrEmpty(sub))
        {
            var adminClient = HttpContext.RequestServices.GetRequiredService<KeycloakAdminClient>();
            var keycloakUser = await adminClient.GetUserByIdAsync(sub);

            if (keycloakUser != null)
            {
                username = keycloakUser.Username;
                email = keycloakUser.Email ?? "";
                fullName = keycloakUser.FullName;
                roles = keycloakUser.Roles;
            }
        }

        // Fallback: roller token'dan
        if (!roles.Any())
        {
            var realmAccess = jwtToken.Claims.FirstOrDefault(c => c.Type == "realm_access")?.Value;
            if (realmAccess != null)
            {
                using var doc = System.Text.Json.JsonDocument.Parse(realmAccess);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    roles = rolesElement.EnumerateArray()
                        .Select(r => r.GetString()!)
                        .Where(r => r is not "offline_access" and not "uma_authorization" and not "default-roles-cardmerchant")
                        .ToList();
                }
            }
        }

        return Ok(new LoginResponse
        {
            Token = json.AccessToken,
            RefreshToken = json.RefreshToken,
            Username = username,
            Email = email,
            FullName = string.IsNullOrEmpty(fullName) ? username : fullName,
            Roles = roles,
            ExpiresAt = DateTime.UtcNow.AddSeconds(json.ExpiresIn)
        });
    }

    /// <summary>
    /// Mevcut kullanıcı bilgisi
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        var authProvider = _configuration["AuthProvider"] ?? "Local";

        if (authProvider == "Keycloak")
        {
            var token = HttpContext.Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedException();

            // Userinfo endpoint'inden sub al
            string? sub = null;
            var client = _httpClientFactory.CreateClient();
            var userinfoEndpoint = $"{_configuration["Keycloak:Authority"]}/protocol/openid-connect/userinfo";
            var userinfoRequest = new HttpRequestMessage(HttpMethod.Get, userinfoEndpoint);
            userinfoRequest.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var userinfoResponse = await client.SendAsync(userinfoRequest);
            if (userinfoResponse.IsSuccessStatusCode)
            {
                var content = await userinfoResponse.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(content);
                sub = doc.RootElement.GetProperty("sub").GetString();
            }

            if (string.IsNullOrEmpty(sub))
                throw new UnauthorizedException();

            // Admin API ile kullanıcı detaylarını al
            var adminClient = HttpContext.RequestServices.GetRequiredService<KeycloakAdminClient>();
            var keycloakUser = await adminClient.GetUserByIdAsync(sub);

            if (keycloakUser == null)
                throw new UnauthorizedException();

            return Ok(new
            {
                Id = keycloakUser.Id,
                Username = keycloakUser.Username,
                Email = keycloakUser.Email ?? "",
                FullName = keycloakUser.FullName,
                Roles = keycloakUser.Roles,
                IsActive = keycloakUser.Enabled
            });
        }

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            throw new UnauthorizedException();

        var user = await _authService.GetUserByUsernameAsync(username);
        return Ok(HandleNotFound(user, "Kullanıcı"));
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

// ══════════════════════════════════════════════════════════════
// REQUEST / RESPONSE MODELS
// ══════════════════════════════════════════════════════════════

/// <summary>
/// Keycloak token endpoint response
/// </summary>
public class KeycloakTokenResponse
{
    [System.Text.Json.Serialization.JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = null!;

    [System.Text.Json.Serialization.JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; set; }

    [System.Text.Json.Serialization.JsonPropertyName("token_type")]
    public string TokenType { get; set; } = null!;
}

/// <summary>
/// Refresh token request
/// </summary>
public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

/// <summary>
/// Logout request
/// </summary>
public class LogoutRequest
{
    public string RefreshToken { get; set; } = null!;
}