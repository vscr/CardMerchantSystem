using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Enums;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.API.Auth.Services;

public class UserService : IUserService
{
    private readonly AuthDbContext _context;

    public UserService(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync(cancellationToken);

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDetailDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return null;

        return MapToDetailDto(user);
    }

    public async Task<UserDto?> CreateUserAsync(CreateUserDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        // Kullanıcı adı veya email kontrolü
        var exists = await _context.Users
            .AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email, cancellationToken);

        if (exists)
            return null;

        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            FullName = dto.FullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        // Rolleri ekle
        foreach (var roleName in dto.Roles)
        {
            if (Enum.TryParse<SystemRole>(roleName, out var systemRole))
            {
                user.UserRoles.Add(new UserRoleEntity
                {
                    UserId = user.Id,
                    RoleId = (int)systemRole,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = createdBy
                });
            }
        }

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // Rolleri yükle
        await _context.Entry(user)
            .Collection(u => u.UserRoles)
            .Query()
            .Include(ur => ur.Role)
            .LoadAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return null;

        // Email değişiyorsa, başka kullanıcıda var mı kontrol et
        if (user.Email != dto.Email)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email && u.Id != id, cancellationToken);

            if (emailExists)
                return null;
        }

        user.Email = dto.Email;
        user.FullName = dto.FullName;
        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<bool> ChangePasswordAsync(Guid id, ChangePasswordDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<UserDto?> UpdateUserRolesAsync(Guid id, UpdateUserRolesDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return null;

        // Mevcut rolleri temizle
        user.UserRoles.Clear();

        // Yeni rolleri ekle
        foreach (var roleName in dto.Roles)
        {
            if (Enum.TryParse<SystemRole>(roleName, out var systemRole))
            {
                user.UserRoles.Add(new UserRoleEntity
                {
                    UserId = user.Id,
                    RoleId = (int)systemRole,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = updatedBy
                });
            }
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Rolleri tekrar yükle
        await _context.Entry(user)
            .Collection(u => u.UserRoles)
            .Query()
            .Include(ur => ur.Role)
            .LoadAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return false;

        // Admin kullanıcısı silinemez
        if (user.Username == "admin")
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> ToggleUserStatusAsync(Guid id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return false;

        // Admin kullanıcısı deaktif edilemez
        if (user.Username == "admin")
            return false;

        user.IsActive = !user.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserDto MapToDto(UserEntity entity)
    {
        return new UserDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            FullName = entity.FullName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            LastLoginAt = entity.LastLoginAt,
            Roles = entity.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }

    private static UserDetailDto MapToDetailDto(UserEntity entity)
    {
        return new UserDetailDto
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            FullName = entity.FullName,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            LastLoginAt = entity.LastLoginAt,
            Roles = entity.UserRoles.Select(ur => ur.Role.Name).ToList(),
            RoleDetails = entity.UserRoles.Select(ur => new UserRoleDto
            {
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name,
                RoleDisplayName = ur.Role.DisplayName,
                AssignedAt = ur.AssignedAt,
                AssignedBy = ur.AssignedBy
            }).ToList()
        };
    }
}