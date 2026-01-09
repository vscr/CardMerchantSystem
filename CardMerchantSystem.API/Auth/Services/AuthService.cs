using CardMerchantSystem.API.Auth.Models;

namespace CardMerchantSystem.API.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;

    // Demo kullanıcıları - Production'da veritabanından gelecek
    private static readonly List<User> _users = new()
    {
        new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            Role = Roles.Admin,
            FullName = "Sistem Yöneticisi",
            IsActive = true
        },
        new User
        {
            Id = 2,
            Username = "operator",
            PasswordHash = HashPassword("operator123"),
            Role = Roles.Operator,
            FullName = "Operatör Kullanıcı",
            IsActive = true
        },
        new User
        {
            Id = 3,
            Username = "viewer",
            PasswordHash = HashPassword("viewer123"),
            Role = Roles.Viewer,
            FullName = "Görüntüleyici Kullanıcı",
            IsActive = true
        }
    };

    public AuthService(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    public Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = _users.FirstOrDefault(u =>
            u.Username == request.Username &&
            u.IsActive);

        if (user == null)
            return Task.FromResult<LoginResponse?>(null);

        if (!VerifyPassword(request.Password, user.PasswordHash))
            return Task.FromResult<LoginResponse?>(null);

        var token = _jwtService.GenerateToken(user);

        var response = new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddHours(8)
        };

        return Task.FromResult<LoginResponse?>(response);
    }

    public Task<User?> GetUserByUsernameAsync(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username == username && u.IsActive);
        return Task.FromResult(user);
    }

    private static string HashPassword(string password)
    {
        // Basit hash - Production'da BCrypt kullanılacak
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}