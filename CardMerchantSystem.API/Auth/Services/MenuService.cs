using CardMerchantSystem.API.Auth.Constants;
using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.API.Auth.Services;

public class MenuService : IMenuService
{
    private readonly AuthDbContext _context;

    public MenuService(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<MenuDto>> GetAllMenusAsync(CancellationToken cancellationToken = default)
    {
        var menus = await _context.Menus
            .Include(m => m.Claims)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        return menus.Select(MapToDto).ToList();
    }

    public async Task<List<MenuDto>> GetMenuTreeAsync(CancellationToken cancellationToken = default)
    {
        var menus = await _context.Menus
            .Include(m => m.Claims)
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        return BuildMenuTree(menus, null);
    }

    public async Task<List<MenuDto>> GetMenusByUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var menus = await _context.Menus
            .Include(m => m.Claims)
            .Where(m => m.IsActive && m.IsVisible)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        // Kullanıcının erişebildiği menüleri filtrele
        var accessibleMenus = menus.Where(m => CanAccessMenu(m, user)).ToList();

        // Erişilebilir menü ID'lerini topla
        var accessibleIds = new HashSet<Guid>(accessibleMenus.Select(m => m.Id));

        // Parent'ları da ekle (duplicate olmadan)
        foreach (var menu in accessibleMenus.ToList())
        {
            AddParentMenus(menu, menus, accessibleIds);
        }

        // Sadece erişilebilir ID'lere sahip menüleri al
        var finalMenus = menus
            .Where(m => accessibleIds.Contains(m.Id))
            .OrderBy(m => m.DisplayOrder)
            .ToList();

        return BuildMenuTree(finalMenus, null);
    }

    private void AddParentMenus(MenuEntity menu, List<MenuEntity> allMenus, HashSet<Guid> accessibleIds)
    {
        if (!menu.ParentId.HasValue)
            return;

        var parent = allMenus.FirstOrDefault(m => m.Id == menu.ParentId.Value);
        if (parent == null)
            return;

        // Zaten ekliyse tekrar ekleme
        if (accessibleIds.Contains(parent.Id))
            return;

        accessibleIds.Add(parent.Id);

        // Recursive olarak parent'ın parent'ını da ekle
        AddParentMenus(parent, allMenus, accessibleIds);
    }

    public async Task<MenuDto?> GetMenuByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var menu = await _context.Menus
            .Include(m => m.Claims)
            .Include(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return menu == null ? null : MapToDto(menu);
    }

    public async Task<MenuDto> CreateMenuAsync(CreateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var menu = new MenuEntity
        {
            Id = Guid.NewGuid(),
            ParentId = dto.ParentId,
            Name = dto.Name.ToLowerInvariant().Replace(" ", "-"),
            Title = dto.Title,
            Icon = dto.Icon,
            Path = dto.Path,
            DisplayOrder = dto.DisplayOrder,
            IsActive = true,
            IsVisible = dto.IsVisible,
            CreatedAt = DateTime.UtcNow
        };

        // Policy'leri ekle
        foreach (var policy in dto.Policies)
        {
            menu.Claims.Add(new MenuClaimEntity
            {
                Id = Guid.NewGuid(),
                MenuId = menu.Id,
                ClaimType = "Policy",
                ClaimValue = policy
            });
        }

        await _context.Menus.AddAsync(menu, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(menu);
    }

    public async Task<MenuDto?> UpdateMenuAsync(Guid id, UpdateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var menu = await _context.Menus
            .Include(m => m.Claims)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (menu == null)
            return null;

        menu.Title = dto.Title;
        menu.Icon = dto.Icon;
        menu.Path = dto.Path;
        menu.DisplayOrder = dto.DisplayOrder;
        menu.IsActive = dto.IsActive;
        menu.IsVisible = dto.IsVisible;
        menu.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(menu);
    }

    public async Task<bool> DeleteMenuAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var menu = await _context.Menus
            .Include(m => m.Children)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (menu == null)
            return false;

        if (menu.Children.Any())
            return false; // Alt menüleri olan menü silinemez

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<MenuDto?> AddClaimAsync(Guid menuId, AddMenuClaimDto dto, CancellationToken cancellationToken = default)
    {
        var menu = await _context.Menus
            .Include(m => m.Claims)
            .FirstOrDefaultAsync(m => m.Id == menuId, cancellationToken);

        if (menu == null)
            return null;

        // Aynı claim var mı kontrol et
        if (menu.Claims.Any(c => c.ClaimType == dto.ClaimType && c.ClaimValue == dto.ClaimValue))
            return MapToDto(menu);

        menu.Claims.Add(new MenuClaimEntity
        {
            Id = Guid.NewGuid(),
            MenuId = menuId,
            ClaimType = dto.ClaimType,
            ClaimValue = dto.ClaimValue
        });

        menu.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(menu);
    }

    public async Task<bool> RemoveClaimAsync(Guid menuId, Guid claimId, CancellationToken cancellationToken = default)
    {
        var claim = await _context.MenuClaims
            .FirstOrDefaultAsync(c => c.Id == claimId && c.MenuId == menuId, cancellationToken);

        if (claim == null)
            return false;

        _context.MenuClaims.Remove(claim);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private bool CanAccessMenu(MenuEntity menu, User user)
    {
        // Claim yoksa herkes erişebilir
        if (!menu.Claims.Any())
            return true;

        // Admin her şeye erişebilir
        if (user.Roles.Contains(RoleNames.Admin))
            return true;

        foreach (var claim in menu.Claims)
        {
            if (claim.ClaimType == "Role")
            {
                if (user.Roles.Contains(claim.ClaimValue))
                    return true;
            }
            else if (claim.ClaimType == "Policy")
            {
                if (CheckPolicy(claim.ClaimValue, user))
                    return true;
            }
        }

        return false;
    }

    private bool CheckPolicy(string policy, User user)
    {
        return policy switch
        {
            Policies.AdminOnly => user.Roles.Contains(RoleNames.Admin),
            Policies.ViewerOrAbove => true, // Herkes
            Policies.CardManagement => user.Roles.Any(r => r == RoleNames.Admin || r == RoleNames.CardOperator),
            Policies.MerchantManagement => user.Roles.Any(r => r == RoleNames.Admin || r == RoleNames.MerchantOperator),
            Policies.FinanceManagement => user.Roles.Any(r => r == RoleNames.Admin || r == RoleNames.FinanceOperator),
            Policies.ComplianceManagement => user.Roles.Any(r => r == RoleNames.Admin || r == RoleNames.ComplianceOfficer),
            Policies.CallCenterAccess => user.Roles.Any(r => r == RoleNames.Admin || r == RoleNames.CardOperator || r == RoleNames.CallCenterAgent),
            Policies.WorkOrderManagement => user.Roles.Any(r => r == RoleNames.Admin || r == RoleNames.CardOperator || r == RoleNames.MerchantOperator || r == RoleNames.FinanceOperator || r == RoleNames.CallCenterAgent),
            _ => false
        };
    }

    private List<MenuDto> BuildMenuTree(List<MenuEntity> menus, Guid? parentId)
    {
        return menus
            .Where(m => m.ParentId == parentId)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new MenuDto
            {
                Id = m.Id,
                ParentId = m.ParentId,
                Name = m.Name,
                Title = m.Title,
                Icon = m.Icon,
                Path = m.Path,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive,
                IsVisible = m.IsVisible,
                Claims = m.Claims.Select(c => new MenuClaimDto
                {
                    Id = c.Id,
                    ClaimType = c.ClaimType,
                    ClaimValue = c.ClaimValue
                }).ToList(),
                Children = BuildMenuTree(menus, m.Id)
            })
            .ToList();
    }

    private MenuDto MapToDto(MenuEntity entity)
    {
        return new MenuDto
        {
            Id = entity.Id,
            ParentId = entity.ParentId,
            Name = entity.Name,
            Title = entity.Title,
            Icon = entity.Icon,
            Path = entity.Path,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            IsVisible = entity.IsVisible,
            Claims = entity.Claims.Select(c => new MenuClaimDto
            {
                Id = c.Id,
                ClaimType = c.ClaimType,
                ClaimValue = c.ClaimValue
            }).ToList(),
            Children = new()
        };
    }
}