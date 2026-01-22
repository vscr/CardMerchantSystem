using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Enums;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.API.Auth.Services;

public class RoleService : IRoleService
{
    private readonly AuthDbContext _context;

    public RoleService(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _context.Roles
            .Include(r => r.UserRoles)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);

        return roles.Select(r => new RoleDto
        {
            Id = r.Id,
            Name = r.Name,
            DisplayName = r.DisplayName,
            Description = r.Description,
            UserCount = r.UserRoles.Count
        }).ToList();
    }

    public async Task<RoleDetailDto?> GetRoleByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.UserRoles)
            .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return null;

        return new RoleDetailDto
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            UserCount = role.UserRoles.Count,
            Users = role.UserRoles.Select(ur => new RoleUserDto
            {
                UserId = ur.UserId,
                Username = ur.User.Username,
                FullName = ur.User.FullName,
                AssignedAt = ur.AssignedAt,
                AssignedBy = ur.AssignedBy
            }).ToList()
        };
    }

    public async Task<RoleDto?> CreateRoleAsync(CreateRoleDto dto, CancellationToken cancellationToken = default)
    {
        // İsim kontrolü
        var exists = await _context.Roles
            .AnyAsync(r => r.Name == dto.Name, cancellationToken);

        if (exists)
            return null;

        // Yeni ID bul (mevcut max + 1)
        var maxId = await _context.Roles.MaxAsync(r => r.Id, cancellationToken);

        var role = new RoleEntity
        {
            Id = maxId + 1,
            Name = dto.Name,
            DisplayName = dto.DisplayName,
            Description = dto.Description
        };

        await _context.Roles.AddAsync(role, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            UserCount = 0
        };
    }

    public async Task<RoleDto?> UpdateRoleAsync(int id, UpdateRoleDto dto, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return null;

        // Sistem rolleri (1-7) Name değiştirilemez
        role.DisplayName = dto.DisplayName;
        role.Description = dto.Description;

        await _context.SaveChangesAsync(cancellationToken);

        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            UserCount = role.UserRoles.Count
        };
    }

    public async Task<bool> DeleteRoleAsync(int id, CancellationToken cancellationToken = default)
    {
        // Sistem rolleri (1-7) silinemez
        if (id <= 7)
            return false;

        var role = await _context.Roles
            .Include(r => r.UserRoles)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role == null)
            return false;

        // Kullanıcısı olan rol silinemez
        if (role.UserRoles.Any())
            return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task<List<RoleDto>> GetSystemRolesAsync()
    {
        var roles = Enum.GetValues<SystemRole>()
            .Select(r => new RoleDto
            {
                Id = (int)r,
                Name = r.ToString(),
                DisplayName = r switch
                {
                    SystemRole.Admin => "Sistem Yöneticisi",
                    SystemRole.CardOperator => "Kart Operasyon",
                    SystemRole.MerchantOperator => "Üye İşyeri Operasyon",
                    SystemRole.FinanceOperator => "Finans Operasyon",
                    SystemRole.ComplianceOfficer => "Uyum Sorumlusu",
                    SystemRole.CallCenterAgent => "Çağrı Merkezi",
                    SystemRole.Viewer => "Görüntüleyici",
                    _ => r.ToString()
                },
                UserCount = 0
            })
            .ToList();

        return Task.FromResult(roles);
    }
}