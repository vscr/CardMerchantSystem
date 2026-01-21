using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public interface IMenuService
{
    Task<List<MenuDto>> GetAllMenusAsync(CancellationToken cancellationToken = default);
    Task<List<MenuDto>> GetMenuTreeAsync(CancellationToken cancellationToken = default);
    Task<List<MenuDto>> GetMenusByUserAsync(User user, CancellationToken cancellationToken = default);
    Task<MenuDto?> GetMenuByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MenuDto> CreateMenuAsync(CreateMenuDto dto, CancellationToken cancellationToken = default);
    Task<MenuDto?> UpdateMenuAsync(Guid id, UpdateMenuDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteMenuAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MenuDto?> AddClaimAsync(Guid menuId, AddMenuClaimDto dto, CancellationToken cancellationToken = default);
    Task<bool> RemoveClaimAsync(Guid menuId, Guid claimId, CancellationToken cancellationToken = default);
}