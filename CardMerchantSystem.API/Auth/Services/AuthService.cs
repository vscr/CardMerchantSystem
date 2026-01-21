using CardMerchantSystem.API.Auth.Entities;
using CardMerchantSystem.API.Auth.Enums;
using CardMerchantSystem.API.Auth.Models;
using CardMerchantSystem.API.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CardMerchantSystem.API.Auth.Services;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;

    public AuthService(AuthDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var userEntity = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive);

        if (userEntity == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, userEntity.PasswordHash))
            return null;

        // Son giriş zamanını güncelle
        userEntity.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var user = MapToModel(userEntity);
        var token = _jwtService.GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Roles = user.Roles,
            ExpiresAt = DateTime.UtcNow.AddHours(8)
        };
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        var userEntity = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        return userEntity == null ? null : MapToModel(userEntity);
    }

    public async Task<bool> CreateUserAsync(string username, string email, string password, string fullName, List<string> roles)
    {
        // Kullanıcı adı veya email kontrolü
        var exists = await _context.Users.AnyAsync(u => u.Username == username || u.Email == email);
        if (exists)
            return false;

        var userEntity = new UserEntity
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            FullName = fullName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(userEntity);

        // Rolleri ekle
        foreach (var roleName in roles)
        {
            if (Enum.TryParse<SystemRole>(roleName, out var systemRole))
            {
                userEntity.UserRoles.Add(new UserRoleEntity
                {
                    UserId = userEntity.Id,
                    RoleId = (int)systemRole,
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "System"
                });
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    private static User MapToModel(UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            FullName = entity.FullName,
            IsActive = entity.IsActive,
            Roles = entity.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}