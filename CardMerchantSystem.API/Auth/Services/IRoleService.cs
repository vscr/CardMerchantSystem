using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public interface IRoleService
{
    Task<List<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default);
    Task<RoleDetailDto?> GetRoleByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<RoleDto?> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default);
    Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteRoleAsync(int id, CancellationToken cancellationToken = default);
    Task<List<RoleDto>> GetSystemRolesAsync();
}