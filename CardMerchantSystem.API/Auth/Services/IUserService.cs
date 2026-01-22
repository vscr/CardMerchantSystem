using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserDetailDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task<UserDto?> UpdateUserRolesAsync(Guid id, UpdateUserRolesDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ToggleUserStatusAsync(Guid id, string updatedBy, CancellationToken cancellationToken = default);
}