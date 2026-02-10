using CardMerchantSystem.API.Controllers;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CardMerchantSystem.API.Auth.Services;

/// <summary>
/// Keycloak Admin REST API client.
/// Kullanıcı CRUD, rol atama ve sorgulama işlemlerini
/// Keycloak üzerinden gerçekleştirir.
/// 
/// API Docs: https://www.keycloak.org/docs-api/26.0/rest-api/
/// </summary>
public class KeycloakAdminClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KeycloakAdminClient> _logger;

    private string? _adminToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public KeycloakAdminClient(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<KeycloakAdminClient> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    private string BaseUrl => _configuration["Keycloak:AdminApi:BaseUrl"]!;
    private string Realm => _configuration["Keycloak:AdminApi:Realm"]!;
    private string AdminUrl => $"{BaseUrl}/admin/realms/{Realm}";

    // ══════════════════════════════════════════════════════════════
    // TOKEN MANAGEMENT
    // ══════════════════════════════════════════════════════════════

    /// <summary>
    /// Master realm üzerinden admin token alır ve cache'ler.
    /// </summary>
    private async Task EnsureTokenAsync()
    {
        if (_adminToken != null && DateTime.UtcNow < _tokenExpiry)
            return;

        var tokenEndpoint = $"{BaseUrl}/realms/master/protocol/openid-connect/token";

        var request = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = "admin-cli",
            ["username"] = _configuration["Keycloak:AdminApi:AdminUsername"]!,
            ["password"] = _configuration["Keycloak:AdminApi:AdminPassword"]!
        };

        var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(request));
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>();
        _adminToken = json!.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(json.ExpiresIn - 30); // 30 sn buffer

        _logger.LogDebug("Keycloak admin token alındı, expiry: {Expiry}", _tokenExpiry);
    }

    private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string url)
    {
        await EnsureTokenAsync();
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);
        return request;
    }

    // ══════════════════════════════════════════════════════════════
    // USER OPERATIONS
    // ══════════════════════════════════════════════════════════════

    /// <summary>
    /// Keycloak'ta yeni kullanıcı oluşturur ve belirtilen rolleri atar.
    /// </summary>
    public async Task<(bool Success, string? UserId, string? Error)> CreateUserAsync(
        string username, string email, string password, string firstName, string lastName, List<string> roles)
    {
        try
        {
            var userRepresentation = new
            {
                username,
                email,
                firstName,
                lastName,
                enabled = true,
                emailVerified = true,
                credentials = new[]
                {
                    new { type = "password", value = password, temporary = false }
                }
            };

            var request = await CreateRequestAsync(HttpMethod.Post, $"{AdminUrl}/users");
            request.Content = new StringContent(
                JsonSerializer.Serialize(userRepresentation),
                Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                return (false, null, "Kullanıcı adı veya email zaten mevcut");

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Keycloak user create failed: {Error}", error);
                return (false, null, $"Keycloak hatası: {response.StatusCode}");
            }

            // Location header'dan user ID al
            var locationHeader = response.Headers.Location?.ToString();
            var userId = locationHeader?.Split('/').Last();

            // Rolleri ata
            if (roles.Any() && userId != null)
            {
                await AssignRealmRolesAsync(userId, roles);
            }

            _logger.LogInformation("Keycloak kullanıcı oluşturuldu: {Username}, Roller: {Roles}",
                username, string.Join(", ", roles));

            return (true, userId, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Keycloak user create exception: {Username}", username);
            return (false, null, ex.Message);
        }
    }

    /// <summary>
    /// Tüm kullanıcıları listeler (sayfalı).
    /// </summary>
    public async Task<List<KeycloakUserDto>> GetUsersAsync(int first = 0, int max = 50, string? search = null)
    {
        var url = $"{AdminUrl}/users?first={first}&max={max}";
        if (!string.IsNullOrEmpty(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        var request = await CreateRequestAsync(HttpMethod.Get, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var users = await response.Content.ReadFromJsonAsync<List<KeycloakUserDto>>() ?? new();

        // Her kullanıcı için rolleri al
        foreach (var user in users)
        {
            user.Roles = await GetUserRealmRolesAsync(user.Id);
        }

        return users;
    }

    /// <summary>
    /// Kullanıcıyı ID ile getirir.
    /// </summary>
    public async Task<KeycloakUserDto?> GetUserByIdAsync(string userId)
    {
        var request = await CreateRequestAsync(HttpMethod.Get, $"{AdminUrl}/users/{userId}");
        var response = await _httpClient.SendAsync(request);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<KeycloakUserDto>();
        if (user != null)
        {
            user.Roles = await GetUserRealmRolesAsync(user.Id);
        }

        return user;
    }

    /// <summary>
    /// Kullanıcı bilgilerini günceller.
    /// </summary>
    public async Task<bool> UpdateUserAsync(string userId, string? email, string? firstName, string? lastName, bool? enabled)
    {
        var current = await GetUserByIdAsync(userId);
        if (current == null) return false;

        var update = new
        {
            email = email ?? current.Email,
            firstName = firstName ?? current.FirstName,
            lastName = lastName ?? current.LastName,
            enabled = enabled ?? current.Enabled
        };

        var request = await CreateRequestAsync(HttpMethod.Put, $"{AdminUrl}/users/{userId}");
        request.Content = new StringContent(
            JsonSerializer.Serialize(update),
            Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Kullanıcıyı deaktif eder (soft delete).
    /// </summary>
    public async Task<bool> DisableUserAsync(string userId)
    {
        return await UpdateUserAsync(userId, enabled: false, email: null, firstName: null, lastName: null);
    }

    /// <summary>
    /// Kullanıcının şifresini sıfırlar.
    /// </summary>
    public async Task<bool> ResetPasswordAsync(string userId, string newPassword)
    {
        var credential = new
        {
            type = "password",
            value = newPassword,
            temporary = false
        };

        var request = await CreateRequestAsync(HttpMethod.Put, $"{AdminUrl}/users/{userId}/reset-password");
        request.Content = new StringContent(
            JsonSerializer.Serialize(credential),
            Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    // ══════════════════════════════════════════════════════════════
    // ROLE OPERATIONS
    // ══════════════════════════════════════════════════════════════

    /// <summary>
    /// Realm'deki tüm rolleri listeler.
    /// </summary>
    public async Task<List<KeycloakRoleDto>> GetRealmRolesAsync()
    {
        var request = await CreateRequestAsync(HttpMethod.Get, $"{AdminUrl}/roles");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRoleDto>>() ?? new();

        // Keycloak default rollerini filtrele
        return roles.Where(r =>
            r.Name is not "offline_access" and not "uma_authorization" and not "default-roles-cardmerchant")
            .ToList();
    }

    /// <summary>
    /// Kullanıcının realm rollerini getirir.
    /// </summary>
    public async Task<List<string>> GetUserRealmRolesAsync(string userId)
    {
        var request = await CreateRequestAsync(HttpMethod.Get, $"{AdminUrl}/users/{userId}/role-mappings/realm");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var roles = await response.Content.ReadFromJsonAsync<List<KeycloakRoleDto>>() ?? new();

        return roles
            .Where(r => r.Name is not "offline_access" and not "uma_authorization" and not "default-roles-cardmerchant")
            .Select(r => r.Name)
            .ToList();
    }

    /// <summary>
    /// Kullanıcıya realm rolleri atar.
    /// </summary>
    public async Task<bool> AssignRealmRolesAsync(string userId, List<string> roleNames)
    {
        // Önce tüm realm rollerini al (ID lazım)
        var allRoles = await GetRealmRolesAsync();
        var rolesToAssign = allRoles
            .Where(r => roleNames.Contains(r.Name, StringComparer.OrdinalIgnoreCase))
            .Select(r => new { id = r.Id, name = r.Name })
            .ToList();

        if (!rolesToAssign.Any())
            return false;

        var request = await CreateRequestAsync(HttpMethod.Post, $"{AdminUrl}/users/{userId}/role-mappings/realm");
        request.Content = new StringContent(
            JsonSerializer.Serialize(rolesToAssign),
            Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    /// <summary>
    /// Toplam kullanıcı sayısını döner.
    /// </summary>
    public async Task<int> GetUserCountAsync()
    {
        var request = await CreateRequestAsync(HttpMethod.Get, $"{AdminUrl}/users/count");
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var count = await response.Content.ReadFromJsonAsync<int>();
        return count;
    }
}

// ══════════════════════════════════════════════════════════════
// DTOs
// ══════════════════════════════════════════════════════════════

public class KeycloakUserDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("username")]
    public string Username { get; set; } = null!;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("firstName")]
    public string? FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string? LastName { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("emailVerified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("createdTimestamp")]
    public long CreatedTimestamp { get; set; }

    // Admin API'den ayrı çağrıyla dolduruluyor
    [JsonIgnore]
    public List<string> Roles { get; set; } = new();

    // Helper
    public string FullName => $"{FirstName} {LastName}".Trim();
}

public class KeycloakRoleDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}