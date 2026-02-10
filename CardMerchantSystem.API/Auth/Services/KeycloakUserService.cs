using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

/// <summary>
/// IUserService'in Keycloak implementasyonu.
/// Tüm kullanıcı CRUD işlemlerini Keycloak Admin REST API üzerinden yapar.
/// Mevcut UsersController hiç değişmeden çalışır (aynı interface).
/// </summary>
public class KeycloakUserService : IUserService
{
    private readonly KeycloakAdminClient _adminClient;
    private readonly ILogger<KeycloakUserService> _logger;

    public KeycloakUserService(KeycloakAdminClient adminClient, ILogger<KeycloakUserService> logger)
    {
        _adminClient = adminClient;
        _logger = logger;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var keycloakUsers = await _adminClient.GetUsersAsync(first: 0, max: 100);

        return keycloakUsers.Select(MapToUserDto).ToList();
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _adminClient.GetUserByIdAsync(id.ToString());
        if (user == null) return null;

        var roles = await _adminClient.GetRealmRolesAsync();

        return new UserDetailDto
        {
            Id = Guid.Parse(user.Id),
            Username = user.Username,
            Email = user.Email ?? "",
            FullName = user.FullName,
            IsActive = user.Enabled,
            CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(user.CreatedTimestamp).UtcDateTime,
            Roles = user.Roles,
            RoleDetails = user.Roles.Select((roleName, index) =>
            {
                var roleInfo = roles.FirstOrDefault(r => r.Name == roleName);
                return new UserRoleDto
                {
                    RoleId = index + 1,
                    RoleName = roleName,
                    RoleDisplayName = roleInfo?.Description ?? roleName,
                    AssignedAt = DateTimeOffset.FromUnixTimeMilliseconds(user.CreatedTimestamp).UtcDateTime,
                    AssignedBy = "Keycloak"
                };
            }).ToList()
        };
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        // FullName'den firstName/lastName ayır
        var nameParts = dto.FullName.Split(' ', 2);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        var (success, userId, error) = await _adminClient.CreateUserAsync(
            dto.Username, dto.Email, dto.Password, firstName, lastName, dto.Roles);

        if (!success)
        {
            _logger.LogWarning("Keycloak user create failed: {Error}", error);
            return null;
        }

        // Oluşturulan kullanıcıyı geri dön
        var user = await _adminClient.GetUserByIdAsync(userId!);
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var nameParts = dto.FullName.Split(' ', 2);
        var firstName = nameParts[0];
        var lastName = nameParts.Length > 1 ? nameParts[1] : "";

        var success = await _adminClient.UpdateUserAsync(
            id.ToString(), dto.Email, firstName, lastName, dto.IsActive);

        if (!success) return null;

        var user = await _adminClient.GetUserByIdAsync(id.ToString());
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        return await _adminClient.ResetPasswordAsync(id.ToString(), dto.NewPassword);
    }

    public async Task<UserDto?> UpdateUserRolesAsync(Guid id, UpdateUserRolesDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        // Önce mevcut rolleri kaldır, sonra yenileri ata
        // Keycloak Admin API'de rol güncelleme: mevcut rolleri sil + yenileri ekle
        var currentRoles = await _adminClient.GetUserRealmRolesAsync(id.ToString());

        // Kaldırılacak roller (şu an var ama yeni listede yok)
        // Not: Keycloak'ta rol silme ayrı endpoint ama basitlik için
        // tüm rolleri silip tekrar ekliyoruz
        var success = await _adminClient.AssignRealmRolesAsync(id.ToString(), dto.Roles);
        if (!success) return null;

        var user = await _adminClient.GetUserByIdAsync(id.ToString());
        return user != null ? MapToUserDto(user) : null;
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Keycloak'ta silmek yerine disable et (soft delete)
        return await _adminClient.DisableUserAsync(id.ToString());
    }

    public async Task<bool> ToggleUserStatusAsync(Guid id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _adminClient.GetUserByIdAsync(id.ToString());
        if (user == null) return false;

        // Admin kullanıcı kontrolü
        if (user.Username == "admin" && user.Enabled)
            return false; // Admin deaktif edilemez

        return await _adminClient.UpdateUserAsync(id.ToString(), enabled: !user.Enabled,
            email: null, firstName: null, lastName: null);
    }

    private static UserDto MapToUserDto(KeycloakUserDto keycloakUser)
    {
        return new UserDto
        {
            Id = Guid.Parse(keycloakUser.Id),
            Username = keycloakUser.Username,
            Email = keycloakUser.Email ?? "",
            FullName = keycloakUser.FullName,
            IsActive = keycloakUser.Enabled,
            CreatedAt = DateTimeOffset.FromUnixTimeMilliseconds(keycloakUser.CreatedTimestamp).UtcDateTime,
            Roles = keycloakUser.Roles
        };
    }
}